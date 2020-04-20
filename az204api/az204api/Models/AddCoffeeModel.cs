using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace az204api.Models
{
    public class AddCoffeeModel
    {
        public string Name { get; set; }
        public string Roastery { get; set; }
        public string Origin { get; set; }
        public string Region { get; set; }
        public int AltitudeMin { get; set; }
        public int AltitudeMax { get; set; }
        public string Process { get; set; }
        public string BrewingMethod { get; set; }
    }
}
