using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using MonaPilot.Console.Models;

namespace MonaPilot.Console.Services
{
    public class RabbitMqEventConsumer
    {
        private readonly IConnection _connection;
        private readonly string _queueName = "product-requests";
        private readonly IProductService _productService;
        private readonly ILogger _logger;
        private const int MAX_RETRIES = 3;

        public RabbitMqEventConsumer(IConnection connection, IProductService productService, ILogger logger)
        {
            _connection = connection;
            _productService = productService;
            _logger = logger;
        }

        public async Task StartListeningAsync(CancellationToken cancellationToken)
        {
            using (var channel = _connection.CreateModel())
            {
                // Kuyruðu deklarasyonu
                channel.QueueDeclare(
                    queue: _queueName,
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);

                // QoS: Bir seferde sadece 1 mesaj al (yük balanslamasý)
                channel.BasicQos(0, 1, false);

                var consumer = new AsyncEventingBasicConsumer(channel);
                int retryCount = 0;

                consumer.Received += async (model, ea) =>
                {
                    try
                    {
                        retryCount = 0;  // Reset retry counter

                        // Mesajý al ve çöz
                        var body = ea.Body.ToArray();
                        var message = Encoding.UTF8.GetString(body);
                        var @event = JsonConvert.DeserializeObject<ProductRequestEvent>(message);

                        // Null check
                        if (@event == null)
                        {
                            _logger.LogError("? Event deserialize edilemedi (null object)");
                            channel.BasicNack(ea.DeliveryTag, false, false);
                            return;
                        }

                        _logger.LogInformation($"?? [Event Alýndý] Request ID: {@event.BudgetRequestId}, Kiþi: {@event.PersonName}, Bütçe: {@event.Budget}, Tür: {@event.ProductType}");

                        // Ürün iþle
                        await _productService.ProcessProductRequestAsync(@event);

                        // Baþarýlý - Kuyruðundan çýkar
                        channel.BasicAck(ea.DeliveryTag, false);
                        _logger.LogInformation($"? Ýþlem baþarýlý - Request ID: {@event.BudgetRequestId}");
                    }
                    catch (Exception ex)
                    {
                        retryCount++;
                        _logger.LogError(ex, $"? Hata (Deneme {retryCount}/{MAX_RETRIES}): {ex.Message}");

                        // Retry mantýðý
                        if (retryCount < MAX_RETRIES)
                        {
                            _logger.LogWarning($"? Mesaj tekrar denenecek...");
                            channel.BasicNack(ea.DeliveryTag, false, true);  // Requeue: true
                        }
                        else
                        {
                            _logger.LogError($"?? Max retry ({MAX_RETRIES}) aþýldý, mesaj atýlýyor");
                            channel.BasicNack(ea.DeliveryTag, false, false);  // Requeue: false (dead letter)
                        }
                    }
                };

                // Kuyruðu dinle
                channel.BasicConsume(queue: _queueName, autoAck: false, consumer: consumer);
                _logger.LogInformation($"? Kuyruk dinleniyor: {_queueName}");
                _logger.LogInformation($"?? QoS: 1 mesaj, Max Retry: {MAX_RETRIES}");

                // Graceful shutdown
                while (!cancellationToken.IsCancellationRequested)
                {
                    await Task.Delay(1000, cancellationToken);
                }

                _logger.LogInformation("?? Kuyruk dinleme durduruldu");
            }
        }
    }
}

