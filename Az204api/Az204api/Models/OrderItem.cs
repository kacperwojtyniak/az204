using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Az204api.Models
{
    public class OrderItem
    {
        public string Id { get; set; }
        public string CoffeeId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }

        public OrderItem()
        {

        }
        public OrderItem(CoffeeModel coffee, int quantity)
        {
            this.Id = Guid.NewGuid().ToString();
            this.CoffeeId = coffee.Id;
            this.UnitPrice = coffee.UnitPrice;
            this.Quantity = quantity;

        }

        
    }
}
