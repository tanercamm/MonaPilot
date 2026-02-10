using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using MonaPilot.Console.Services;

namespace MonaPilot.Console
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            // Logger oluştur
            var loggerFactory = new LoggerFactory();
            loggerFactory.AddProvider(new ConsoleLoggerProvider());
            var logger = loggerFactory.CreateLogger("MonaPilot.Console");

            logger.LogInformation("=== MonaPilot Console Uygulaması Başlatılıyor ===");

            try
            {
                // RabbitMQ bağlantısı
                var rabbitMqHost = "localhost";
                var connectionFactory = new ConnectionFactory()
                {
                    HostName = rabbitMqHost,
                    DispatchConsumersAsync = true
                };

                IConnection connection = null;
                try
                {
                    connection = connectionFactory.CreateConnection();
                    logger.LogInformation("✅ RabbitMQ'ya bağlanıldı");
                }
                catch (Exception ex)
                {
                    logger.LogError($"❌ RabbitMQ bağlantı hatası: {ex.Message}");
                    logger.LogError("RabbitMQ'nun çalışıp çalışmadığını kontrol edin!");
                    logger.LogError("Çalıştır: docker-compose up -d");
                    return;
                }

                // Servisler
                var productService = new ProductService(logger);
                var consumer = new RabbitMqEventConsumer(connection, productService, logger);

                // CancellationToken - Ctrl+C ile durdurabilmesi için
                var cts = new CancellationTokenSource();
                System.Console.CancelKeyPress += (s, e) =>
                {
                    e.Cancel = true;
                    cts.Cancel();
                    logger.LogInformation("=== Uygulama kapatılıyor ===");
                };

                // Kuyrağu dinle
                await consumer.StartListeningAsync(cts.Token);
            }
            catch (Exception ex)
            {
                logger.LogError($"❌ Hata oluştu: {ex.Message}");
                logger.LogError(ex.StackTrace);
                System.Console.WriteLine(ex);
            }
        }
    }
}
