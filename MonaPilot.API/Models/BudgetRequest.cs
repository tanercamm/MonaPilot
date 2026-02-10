namespace MonaPilot.API.Models
{
    public class BudgetRequest
    {
        public int Id { get; set; }
        public string PersonName { get; set; }
        public decimal Budget { get; set; }
        public string ProductType { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string Status { get; set; } = "Pending"; // Pending, Completed, Failed
    }
}
