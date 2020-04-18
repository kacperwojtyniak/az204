using az204api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace az204api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CoffeeController : ControllerBase
    {
        private const string DATABASE_ID = "Az204";
        private const string CONTAINER_ID = "Coffees";
        private readonly Container container;
        private readonly ILogger<CoffeeController> _logger;

        public CoffeeController(CosmosClient client, ILogger<CoffeeController> logger)
        {
            this.container = client.GetContainer(DATABASE_ID, CONTAINER_ID);
            _logger = logger;
        }

        [HttpGet("{roastery}")]
        public async Task<QueryResult> Get(string roastery, [FromQuery]string continuationToken = default(string))
        {
            try
            {
                if (!string.IsNullOrEmpty(continuationToken))
                {
                    var tokenBytes = Convert.FromBase64String(continuationToken);
                    continuationToken = UTF8Encoding.UTF8.GetString(tokenBytes);
                }
                var query = $"select * from c where c.roastery = '{roastery}'";                
                var iterator = container.GetItemQueryIterator<CoffeeModel>(query, continuationToken);
                var result = await iterator.ReadNextAsync();                
                return new QueryResult(result.ContinuationToken, result, result.RequestCharge);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get coffes from {roastery}", roastery);
                throw;
            }            
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody]AddCoffeeModel coffee)
        {
            await container.CreateItemAsync(CoffeeModel.Create(coffee), new PartitionKey(coffee.Roastery));
            return Ok();
        }

        [HttpDelete("{coffeeId}/{roaster}")]
        public async Task<IActionResult> Delete(string coffeeId, string roaster)
        {
            await container.DeleteItemAsync<CoffeeModel>(coffeeId, new PartitionKey(roaster));
            return Ok();
        }
    }
}
