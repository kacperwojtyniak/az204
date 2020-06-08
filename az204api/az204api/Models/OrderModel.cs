using System;
using System.Collections.Generic;
using System.Linq;

namespace Az204api.Models
{
    public class OrderModel : Document
    {        
        public string Date { get; set; }
        public decimal TotalAmount { get; set; }
        public OrderStatus Status { get; set; } = OrderStatus.Pending;
        public IEnumerable<OrderItem> Items { get; set; }
        public string Type => "Order";
        public DateTime ApprovalDate { get; set; }
        public OrderModel()
        {

        }
        public OrderModel(IEnumerable<OrderItem> items)
        {            
            this.Date = DateTime.UtcNow.ToShortDateString();
            this.Items = items;
            this.TotalAmount = this.Items.Sum(x => x.UnitPrice * x.Quantity);
        }

    }
}
