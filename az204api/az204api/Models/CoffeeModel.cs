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
        public string Origin { get; set; }
        public string Region { get; set; }
        public int AltitudeMin { get; set; }
        public int AltitudeMax { get; set; }
        public string Process { get; set; }
        public string BrewingMethod { get; set; }

        public static CoffeeModel Create(AddCoffeeModel coffee)
        {
            return new CoffeeModel()
            {
                Id = Guid.NewGuid().ToString(),
                Roastery = coffee.Roastery,
                Origin = coffee.Origin,
                Region = coffee.Region,
                AltitudeMax = coffee.AltitudeMax,
                AltitudeMin = coffee.AltitudeMin,
                Process = coffee.Process,
                BrewingMethod = coffee.BrewingMethod
            };
        }
    }
}
