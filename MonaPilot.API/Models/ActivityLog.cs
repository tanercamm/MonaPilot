namespace MonaPilot.API.Models
{
    public class ActivityLog
    {
        public int Id { get; set; }
        public int BudgetRequestId { get; set; }
        public string PersonName { get; set; }
        public string ProductName { get; set; }
        public decimal ProductPrice { get; set; }
        public int RemainingStock { get; set; }
        public string LogMessage { get; set; }
        public string Status { get; set; } // "Success", "Warning", "Error"
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
