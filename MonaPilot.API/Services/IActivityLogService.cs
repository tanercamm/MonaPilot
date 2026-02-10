using MonaPilot.API.Models;
using MonaPilot.API.Data;

namespace MonaPilot.API.Services
{
    public interface IActivityLogService
    {
        Task LogActivityAsync(int budgetRequestId, string personName, string productName, 
            decimal productPrice, int remainingStock, string message, string status);
        Task<List<ActivityLog>> GetLogsByPersonAsync(string personName);
        Task<List<ActivityLog>> GetAllLogsAsync();
    }

    public class ActivityLogService : IActivityLogService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogPublisher _logPublisher;
        private readonly ILogger<ActivityLogService> _logger;

        public ActivityLogService(ApplicationDbContext dbContext, ILogPublisher logPublisher, ILogger<ActivityLogService> logger)
        {
            _dbContext = dbContext;
            _logPublisher = logPublisher;
            _logger = logger;
        }

        public async Task LogActivityAsync(int budgetRequestId, string personName, string productName,
            decimal productPrice, int remainingStock, string message, string status)
        {
            try
            {
                var log = new ActivityLog
                {
                    BudgetRequestId = budgetRequestId,
                    PersonName = personName,
                    ProductName = productName,
                    ProductPrice = productPrice,
                    RemainingStock = remainingStock,
                    LogMessage = message,
                    Status = status,
                    CreatedAt = DateTime.UtcNow
                };

                _dbContext.ActivityLogs.Add(log);
                await _dbContext.SaveChangesAsync();

                // Log'u RabbitMQ'ya gönder (Console app dinleyebilir)
                await _logPublisher.PublishLogAsync(log);

                _logger.LogInformation($"[LOG KAYDEDILDI] {personName} - {message}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Log kaydý baþarýsýz: {ex.Message}");
            }
        }

        public async Task<List<ActivityLog>> GetLogsByPersonAsync(string personName)
        {
            return await Task.FromResult(
                _dbContext.ActivityLogs
                    .Where(l => l.PersonName == personName)
                    .OrderByDescending(l => l.CreatedAt)
                    .ToList()
            );
        }

        public async Task<List<ActivityLog>> GetAllLogsAsync()
        {
            return await Task.FromResult(
                _dbContext.ActivityLogs
                    .OrderByDescending(l => l.CreatedAt)
                    .ToList()
            );
        }
    }
}
