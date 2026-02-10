using Microsoft.AspNetCore.Mvc;
using MonaPilot.API.Services;

namespace MonaPilot.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthenticationService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthenticationService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            if (string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password))
            {
                return BadRequest(new { message = "Username and password are required" });
            }

            // Demo: Accept any login (production'da database kontrol etmeli)
            if (request.Username == "admin" && request.Password == "admin123")
            {
                var token = _authService.GenerateToken(request.Username);
                _logger.LogInformation($"User logged in: {request.Username}");
                
                return Ok(new
                {
                    token = token,
                    username = request.Username,
                    expiresIn = 3600
                });
            }

            return Unauthorized(new { message = "Invalid credentials" });
        }

        [HttpPost("refresh")]
        public IActionResult RefreshToken([FromBody] RefreshTokenRequest request)
        {
            if (string.IsNullOrEmpty(request.Token))
            {
                return BadRequest("Token required");
            }

            if (_authService.ValidateToken(request.Token))
            {
                var newToken = _authService.GenerateToken("admin");
                return Ok(new { token = newToken });
            }

            return Unauthorized("Invalid token");
        }
    }

    public class LoginRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class RefreshTokenRequest
    {
        public string Token { get; set; }
    }
}
