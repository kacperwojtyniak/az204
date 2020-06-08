using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Az204api.Models
{
    public class CoffeeModel : Document
    {        
        public string Name { get; set; }
        public string Roastery { get; set; }
        public string Origin { get; set; }
        public string Region { get; set; }
        public int AltitudeMin { get; set; }
        public int AltitudeMax { get; set; }
        public string Process { get; set; }
        public string BrewingMethod { get; set; }
        public decimal UnitPrice { get; set; }
        public string ImgUrl { get; set; }
    }
}
