﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace az204api.Models
{
    public class OrderModel
    {

        public string Id { get; set; }
        public string Date { get; set; }
        public decimal TotalAmount { get; set; }
        public OrderStatus Status { get; set; } = OrderStatus.Pending;
        public IEnumerable<OrderItem> Items { get; set; }
        public string Type => "Order";
        public OrderModel()
        {

        }
        public OrderModel(IEnumerable<OrderItem> items)
        {
            this.Id = Guid.NewGuid().ToString();
            this.Date = DateTime.UtcNow.ToShortDateString();
            this.Items = items;
            this.TotalAmount = this.Items.Sum(x => x.UnitPrice * x.Quantity);
        }

    }
}
