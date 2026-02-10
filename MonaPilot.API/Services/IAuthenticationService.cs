using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace MonaPilot.API.Services
{
    public interface IAuthenticationService
    {
        string GenerateToken(string username);
        bool ValidateToken(string token);
    }

    public class JwtAuthenticationService : IAuthenticationService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<JwtAuthenticationService> _logger;

        public JwtAuthenticationService(IConfiguration configuration, ILogger<JwtAuthenticationService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public string GenerateToken(string username)
        {
            var jwtKey = _configuration["Jwt:Key"] ?? "your-secret-key-change-this-in-production";
            var jwtIssuer = _configuration["Jwt:Issuer"] ?? "monapilot";
            var jwtExpireMinutes = int.Parse(_configuration["Jwt:ExpireMinutes"] ?? "60");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, username),
                new Claim(ClaimTypes.Name, username),
                new Claim("type", "user")
            };

            var token = new JwtSecurityToken(
                issuer: jwtIssuer,
                audience: "monapilot-app",
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(jwtExpireMinutes),
                signingCredentials: credentials
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
            _logger.LogInformation($"Token generated for user: {username}");
            return tokenString;
        }

        public bool ValidateToken(string token)
        {
            try
            {
                var jwtKey = _configuration["Jwt:Key"] ?? "your-secret-key-change-this-in-production";
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));

                var tokenHandler = new JwtSecurityTokenHandler();
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = key,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                return validatedToken is not null;
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"Token validation failed: {ex.Message}");
                return false;
            }
        }
    }
}
