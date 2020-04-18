using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using az204api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace az204api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CoffeeController : ControllerBase
    {        
        private readonly ILogger<CoffeeController> _logger;

        public CoffeeController(ILogger<CoffeeController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<CoffeeModel> Get()
        {
            throw new NotImplementedException();
        }

        [HttpPost]
        public IActionResult Add([FromBody]AddCoffeeModel coffee)
        {
            throw new NotImplementedException();
        }

        [HttpDelete("{coffeeId}")]
        public IActionResult Delete(string coffeeId)
        {
            throw new NotImplementedException();
        }
    }
}
