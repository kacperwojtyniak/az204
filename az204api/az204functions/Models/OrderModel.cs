using System.Collections.Generic;

namespace az204functions.Models
{
    public class OrderModel
    {
        public string Id { get; set; }
        public string Date { get; set; }
        public decimal TotalAmount { get; set; }
        public OrderStatus Status { get; set; }
    }
}
