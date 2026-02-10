using MonaPilot.API.Models;
using RabbitMQ.Client;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace MonaPilot.API.Services
{
    public class RabbitMqEventPublisher : IEventPublisher
    {
        private readonly IConnection _connection;
        private readonly ILogger<RabbitMqEventPublisher> _logger;
        private readonly string _queueName = "product-requests";

        public RabbitMqEventPublisher(IConnection connection, ILogger<RabbitMqEventPublisher> logger)
        {
            _connection = connection;
            _logger = logger;
        }

        public async Task PublishProductRequestAsync(ProductRequestEvent @event)
        {
            if (_connection == null)
            {
                _logger.LogWarning("RabbitMQ baglantisi yok, event yayinlanamadi");
                return;
            }

            try
            {
                using (var channel = _connection.CreateModel())
                {
                    channel.QueueDeclare(
                        queue: _queueName,
                        durable: true,
                        exclusive: false,
                        autoDelete: false,
                        arguments: null);

                    var message = JsonSerializer.Serialize(@event);
                    var body = System.Text.Encoding.UTF8.GetBytes(message);

                    channel.BasicPublish(
                        exchange: "",
                        routingKey: _queueName,
                        basicProperties: null,
                        body: body);
                    
                    _logger.LogInformation($"Event yayinlandi - Request ID: {@event.BudgetRequestId}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Event yayinlama hatasi: {ex.Message}");
            }

            await Task.CompletedTask;
        }
    }
}
