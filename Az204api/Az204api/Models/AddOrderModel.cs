﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Az204api.Models
{
    public class AddOrderModel
    {
        public IEnumerable<CoffeeToOrderModel> Coffees { get; set; }
    }
}
