using RabbitMQ.Client;
using System;
using System.Threading.Tasks;

namespace MonaPilot.API.Services
{
    /// <summary>
    /// Singleton RabbitMQ connection yönetimi
    /// </summary>
    public interface IRabbitMqConnection
    {
        IConnection GetConnection();
    }

    public class RabbitMqConnection : IRabbitMqConnection, IDisposable
    {
        private IConnection _connection;
        private readonly IConnectionFactory _connectionFactory;
        private readonly ILogger<RabbitMqConnection> _logger;
        private readonly object _lockObject = new object();

        public RabbitMqConnection(IConnectionFactory connectionFactory, ILogger<RabbitMqConnection> logger)
        {
            _connectionFactory = connectionFactory;
            _logger = logger;
        }

        public IConnection GetConnection()
        {
            if (_connection == null || !_connection.IsOpen)
            {
                lock (_lockObject)
                {
                    if (_connection == null || !_connection.IsOpen)
                    {
                        try
                        {
                            _connection = _connectionFactory.CreateConnection();
                            _logger.LogInformation("? RabbitMQ baðlantýsý baþarýlý");
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError($"? RabbitMQ baðlantý hatasý: {ex.Message}");
                            throw;
                        }
                    }
                }
            }
            return _connection;
        }

        public void Dispose()
        {
            _connection?.Close();
            _connection?.Dispose();
        }
    }
}
