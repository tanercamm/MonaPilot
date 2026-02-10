using Microsoft.AspNetCore.SignalR;

namespace MonaPilot.API.Hubs
{
    public class NotificationHub : Hub
    {
        private readonly ILogger<NotificationHub> _logger;

        public NotificationHub(ILogger<NotificationHub> logger)
        {
            _logger = logger;
        }

        public override async Task OnConnectedAsync()
        {
            _logger.LogInformation($"Client connected: {Context.ConnectionId}");
            await Clients.All.SendAsync("ReceiveMessage", "System", "New user connected");
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            _logger.LogInformation($"Client disconnected: {Context.ConnectionId}");
            await Clients.All.SendAsync("ReceiveMessage", "System", "User disconnected");
            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }

        public async Task NotifyNewRequest(int requestId, string personName, decimal budget, string productType)
        {
            await Clients.All.SendAsync("NewRequest", new
            {
                requestId,
                personName,
                budget,
                productType,
                timestamp = DateTime.UtcNow
            });
        }

        public async Task NotifyRequestCompleted(int requestId, string productName, decimal price, int stock)
        {
            await Clients.All.SendAsync("RequestCompleted", new
            {
                requestId,
                productName,
                price,
                stock,
                timestamp = DateTime.UtcNow
            });
        }

        public async Task UpdateDashboard(object data)
        {
            await Clients.All.SendAsync("DashboardUpdated", data);
        }
    }
}
