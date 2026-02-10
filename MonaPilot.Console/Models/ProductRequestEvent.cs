using System;

namespace MonaPilot.Console.Models
{
    public class ProductRequestEvent
    {
        public int BudgetRequestId { get; set; }
        public string PersonName { get; set; }
        public decimal Budget { get; set; }
        public string ProductType { get; set; }
        public DateTime RequestDate { get; set; }
    }
}
