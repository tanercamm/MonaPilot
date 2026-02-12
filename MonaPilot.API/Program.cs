using Microsoft.EntityFrameworkCore;
using MonaPilot.API.Data;
using MonaPilot.API.Services;
using MonaPilot.API.Hubs;
using RabbitMQ.Client;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddSignalR();  // SIGNALR EKLE

// JWT Authentication
builder.Services.AddScoped<IAuthenticationService, JwtAuthenticationService>();
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer(options =>
    {
        var key = builder.Configuration["Jwt:Key"] ?? "your-super-secret-key-change-this-in-production-minimum-32-characters";
        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(key)),
            ValidateIssuer = false,
            ValidateAudience = false,
            ClockSkew = TimeSpan.Zero
        };
    });
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazor", builder =>
        builder.WithOrigins("http://localhost:5088", "https://localhost:5088",
                            "http://localhost:5281", "https://localhost:5281")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials());
});

// Database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite("Data Source=monapilot.db"));

// RabbitMQ Configuration - OPTIONAL (API RabbitMQ yoksa da çalışacak)
var rabbitMqHost = builder.Configuration["RabbitMq:Host"] ?? "localhost";
var connectionFactory = new ConnectionFactory()
{
    HostName = rabbitMqHost,
    DispatchConsumersAsync = true,
    AutomaticRecoveryEnabled = true,
    NetworkRecoveryInterval = TimeSpan.FromSeconds(10)
};

builder.Services.AddSingleton<IConnectionFactory>(connectionFactory);
builder.Services.AddSingleton<IRabbitMqConnection, RabbitMqConnection>();

// RabbitMQ connection'ı lazy olarak yükleme (startup sırasında bağlanmaya çalışma)
builder.Services.AddSingleton<IConnection>(sp => 
{
    try
    {
        var rabbitmqConn = sp.GetRequiredService<IRabbitMqConnection>();
        return rabbitmqConn.GetConnection();
    }
    catch (Exception ex)
    {
        var logger = sp.GetRequiredService<ILogger<Program>>();
        logger.LogWarning($"⚠️  RabbitMQ bağlantısı başarısız (daha sonra denecek): {ex.Message}");
        return null;
    }
});

builder.Services.AddSingleton<IEventPublisher, RabbitMqEventPublisher>();
builder.Services.AddSingleton<ILogPublisher, RabbitMqLogPublisher>();
builder.Services.AddScoped<IActivityLogService, ActivityLogService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();

var app = builder.Build();

// Initialize Database
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    DbInitializer.Initialize(dbContext);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();
app.UseCors("AllowBlazor");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapHub<NotificationHub>("/hubs/notifications");  // SIGNALR HUB

app.Run();
