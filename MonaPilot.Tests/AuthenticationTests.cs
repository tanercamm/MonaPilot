using Xunit;
using MonaPilot.API.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;

namespace MonaPilot.Tests
{
    public class AuthenticationServiceTests
    {
        private readonly IAuthenticationService _authService;
        private readonly IConfiguration _configuration;
        private readonly Mock<ILogger<JwtAuthenticationService>> _logger;

        public AuthenticationServiceTests()
        {
            var inMemorySettings = new Dictionary<string, string>
            {
                { "Jwt:Key", "your-super-secret-key-change-this-in-production-minimum-32-characters" },
                { "Jwt:Issuer", "monapilot-api" },
                { "Jwt:Audience", "monapilot-app" },
                { "Jwt:ExpireMinutes", "60" }
            };

            _configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            _logger = new Mock<ILogger<JwtAuthenticationService>>();
            _authService = new JwtAuthenticationService(_configuration, _logger.Object);
        }

        [Fact]
        public void GenerateToken_WithValidUsername_ReturnsValidToken()
        {
            // Arrange
            var username = "testuser";

            // Act
            var token = _authService.GenerateToken(username);

            // Assert
            Assert.NotNull(token);
            Assert.NotEmpty(token);
        }

        [Fact]
        public void ValidateToken_WithValidToken_ReturnsTrue()
        {
            // Arrange
            var username = "testuser";
            var token = _authService.GenerateToken(username);

            // Act
            var isValid = _authService.ValidateToken(token);

            // Assert
            Assert.True(isValid);
        }

        [Fact]
        public void ValidateToken_WithInvalidToken_ReturnsFalse()
        {
            // Arrange
            var invalidToken = "invalid.token.here";

            // Act
            var isValid = _authService.ValidateToken(invalidToken);

            // Assert
            Assert.False(isValid);
        }

        [Theory]
        [InlineData("admin")]
        [InlineData("user123")]
        [InlineData("test@email.com")]
        public void GenerateToken_WithDifferentUsernames_GeneratesDifferentTokens(string username)
        {
            // Act
            var token1 = _authService.GenerateToken(username);
            var token2 = _authService.GenerateToken(username);

            // Assert - Tokens should be different due to timestamps
            Assert.NotEqual(token1, token2);
            Assert.NotEmpty(token1);
            Assert.NotEmpty(token2);
        }
    }

    public class ActivityLogTests
    {
        [Fact]
        public void ActivityLog_WithValidData_CreatesSuccessfully()
        {
            // Arrange & Act
            var log = new MonaPilot.API.Models.ActivityLog
            {
                PersonName = "John Doe",
                ProductName = "Laptop",
                ProductPrice = 1200,
                Status = "Success"
            };

            // Assert
            Assert.NotNull(log);
            Assert.Equal("John Doe", log.PersonName);
            Assert.Equal("Success", log.Status);
        }

        [Fact]
        public void ActivityLog_CreationDate_IsSet()
        {
            // Arrange
            var before = DateTime.UtcNow;

            // Act
            var log = new MonaPilot.API.Models.ActivityLog
            {
                PersonName = "Jane Doe",
                Status = "Success"
            };

            var after = DateTime.UtcNow;

            // Assert
            Assert.True(log.CreatedAt >= before && log.CreatedAt <= after);
        }
    }
}
