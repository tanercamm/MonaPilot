namespace MonaPilot.API.Models
{
    public class Order
    {
        public int Id { get; set; }
        public int BudgetRequestId { get; set; }
        public int ProductId { get; set; }
        public string PersonName { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
    }
}
