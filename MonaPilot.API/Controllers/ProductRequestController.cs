using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MonaPilot.API.Data;
using MonaPilot.API.Models;
using MonaPilot.API.Services;

namespace MonaPilot.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]  // JWT token zorunlu
    public class ProductRequestController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IEventPublisher _eventPublisher;
        private readonly IActivityLogService _activityLogService;
        private readonly ILogger<ProductRequestController> _logger;

        public ProductRequestController(
            ApplicationDbContext dbContext,
            IEventPublisher eventPublisher,
            IActivityLogService activityLogService,
            ILogger<ProductRequestController> logger)
        {
            _dbContext = dbContext;
            _eventPublisher = eventPublisher;
            _activityLogService = activityLogService;
            _logger = logger;
        }

        [HttpPost("request")]
        public async Task<ActionResult<BudgetRequest>> CreateProductRequest([FromBody] BudgetRequest request)
        {
            if (string.IsNullOrEmpty(request.PersonName) || request.Budget <= 0)
            {
                return BadRequest("Geçersiz kiþi adý veya bütçe");
            }

            try
            {
                // DB'ye kaydet
                _dbContext.BudgetRequests.Add(request);
                await _dbContext.SaveChangesAsync();

                _logger.LogInformation($"Yeni talep kaydedildi - ID: {request.Id}, Kiþi: {request.PersonName}, Bütçe: {request.Budget}, Tür: {request.ProductType}");

                // Event'i kuyruða gönder
                var productEvent = new ProductRequestEvent
                {
                    BudgetRequestId = request.Id,
                    PersonName = request.PersonName,
                    Budget = request.Budget,
                    ProductType = request.ProductType
                };

                await _eventPublisher.PublishProductRequestAsync(productEvent);

                _logger.LogInformation($"Event yayýnlandý - Request ID: {request.Id}");

                return CreatedAtAction(nameof(GetProductRequest), new { id = request.Id }, request);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Hata oluþtu: {ex.Message}");
                return StatusCode(500, "Ýþlem baþarýsýz");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<BudgetRequest>> GetProductRequest(int id)
        {
            var request = await _dbContext.BudgetRequests.FindAsync(id);
            if (request == null)
            {
                return NotFound();
            }

            return request;
        }

        [HttpGet("all")]
        public ActionResult<List<BudgetRequest>> GetAllRequests()
        {
            return _dbContext.BudgetRequests.ToList();
        }

        [HttpGet("logs/all")]
        public async Task<ActionResult<List<object>>> GetAllLogs()
        {
            var logs = await _activityLogService.GetAllLogsAsync();
            return Ok(logs.Select(l => new
            {
                l.Id,
                l.BudgetRequestId,
                l.PersonName,
                l.ProductName,
                l.ProductPrice,
                l.RemainingStock,
                l.LogMessage,
                l.Status,
                l.CreatedAt
            }).ToList());
        }

        [HttpGet("logs/person/{personName}")]
        public async Task<ActionResult<List<object>>> GetLogsByPerson(string personName)
        {
            var logs = await _activityLogService.GetLogsByPersonAsync(personName);
            return Ok(logs.Select(l => new
            {
                l.Id,
                l.BudgetRequestId,
                l.ProductName,
                l.ProductPrice,
                l.LogMessage,
                l.Status,
                l.CreatedAt
            }).ToList());
        }
    }
}
