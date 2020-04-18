using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace az204api.Models
{
    public class CoffeeModel
    {
        public string Id { get; set; }
        public string Roastery { get; set; }
        public string Country { get; set; }
        public string Region { get; set; }
        public int Altitude { get; set; }
        public string Process { get; set; }
        public string BrewingMethod { get; set; }
    }
}
