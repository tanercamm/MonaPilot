using MonaPilot.API.Models;
using RabbitMQ.Client;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace MonaPilot.API.Services
{
    public interface ILogPublisher
    {
        Task PublishLogAsync(ActivityLog log);
    }

    public class RabbitMqLogPublisher : ILogPublisher
    {
        private readonly IConnection _connection;
        private readonly ILogger<RabbitMqLogPublisher> _logger;
        private readonly string _queueName = "activity-logs";

        public RabbitMqLogPublisher(IConnection connection, ILogger<RabbitMqLogPublisher> logger)
        {
            _connection = connection;
            _logger = logger;
        }

        public async Task PublishLogAsync(ActivityLog log)
        {
            if (_connection == null)
            {
                _logger.LogWarning("RabbitMQ baglantisi yok, log yayinlanamadi");
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

                    var message = JsonSerializer.Serialize(log);
                    var body = System.Text.Encoding.UTF8.GetBytes(message);

                    channel.BasicPublish(
                        exchange: "",
                        routingKey: _queueName,
                        basicProperties: null,
                        body: body);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Log yayinlama hatasi: {ex.Message}");
            }

            await Task.CompletedTask;
        }
    }
}
