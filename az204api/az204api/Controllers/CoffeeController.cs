using az204api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using static az204api.Constants;

namespace az204api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CoffeeController : ControllerBase
    {
        
        private readonly CosmosClient client;
        private readonly ILogger<CoffeeController> _logger;

        public CoffeeController(CosmosClient client, ILogger<CoffeeController> logger)
        {
            this.client = client;
            _logger = logger;
        }

        [HttpGet("roasters")]
        public async Task<QueryResult> GetAllRoasters([FromQuery]string continuationToken = default(string))
        {
            try
            {
                var query = $"SELECT DISTINCT VALUE c.roastery FROM c ORDER BY c.roastery";
                if (!string.IsNullOrEmpty(continuationToken))
                {
                    var tokenBytes = Convert.FromBase64String(continuationToken);
                    continuationToken = UTF8Encoding.UTF8.GetString(tokenBytes);
                }
                var container = client.GetContainer(DATABASE_ID, COFFEES_CONTAINER);
                var iterator = container.GetItemQueryIterator<dynamic>(query, continuationToken);
               
                var result = await iterator.ReadNextAsync();                
                return new QueryResult(result.ContinuationToken, result, result.RequestCharge);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get roasters");
                throw;
            }

        }
        [HttpGet("roastery/{roastery}")]
        public async Task<QueryResult> Get(string roastery, [FromQuery]string continuationToken = default(string))
        {
            try
            {
                var query = $"SELECT * FROM c WHERE c.roastery = '{roastery}'";
                var container = client.GetContainer(DATABASE_ID, COFFEES_CONTAINER);
                return await QueryAsync(query, container, continuationToken: continuationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get coffes from {roastery}", roastery);
                throw;
            }            
        }
        [HttpGet("brewing/{brewingMethod}")]
        public async Task<QueryResult> GetByBrewingMethod(string brewingMethod, [FromQuery]string partitionKey, [FromQuery]string continuationToken = default(string))
        {
            try
            {
                var query = $"SELECT * FROM c";
                var container = client.GetContainer(DATABASE_ID, COFFEESBREWING_CONTAINER);
                return await QueryAsync(query, container, partitionKey, continuationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get coffes");
                throw;
            }
        }
        [HttpGet("origin/{origin}")]
        public async Task<QueryResult> GetByOrigin(string origin, [FromQuery]string partitionKey, [FromQuery]string continuationToken = default(string))
        {
            try
            {
                var query = $"SELECT * FROM c WHERE c.origin = '{origin}'";
                var container = client.GetContainer(DATABASE_ID, COFFEES_CONTAINER);
                return await QueryAsync(query, container, partitionKey, continuationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get coffes");
                throw;
            }
        }

        [HttpGet("{id}")]
        public async Task<QueryResult> GetById(string id)
        {
            try
            {
                var query = $"SELECT * FROM c WHERE c.id = '{id}'";
                var container = client.GetContainer(DATABASE_ID, COFFEES_CONTAINER);
                return await QueryAsync(query, container);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get coffes");
                throw;
            }
        }


        [HttpPost]
        public async Task<IActionResult> Add([FromBody]AddCoffeeModel coffee)
        {
            var container = client.GetContainer(DATABASE_ID, COFFEES_CONTAINER);
            await container.CreateItemAsync(CoffeeModel.Create(coffee), new PartitionKey(coffee.Roastery));
            return Ok();
        }

        [HttpPost("update")]
        public async Task<IActionResult> Update([FromBody]CoffeeModel coffee)
        {
            var container = client.GetContainer(DATABASE_ID, COFFEES_CONTAINER);
            await container.UpsertItemAsync(coffee, new PartitionKey(coffee.Roastery));
            return Ok();
        }

        [HttpDelete("{coffeeId}/{roaster}")]
        public async Task<IActionResult> Delete(string coffeeId, string roaster)
        {
            var container = client.GetContainer(DATABASE_ID, COFFEES_CONTAINER);
            await container.DeleteItemAsync<CoffeeModel>(coffeeId, new PartitionKey(roaster));
            return Ok();
        }

        private async Task<QueryResult> QueryAsync(string query,Container container, string partitionKey = null, string continuationToken = null)
        {
            var queryOptions = string.IsNullOrEmpty(partitionKey) ? null : new QueryRequestOptions() { PartitionKey = new PartitionKey(partitionKey) };
            if (!string.IsNullOrEmpty(continuationToken))
            {
                var tokenBytes = Convert.FromBase64String(continuationToken);
                continuationToken = UTF8Encoding.UTF8.GetString(tokenBytes);
            }
            
            var iterator = container.GetItemQueryIterator<CoffeeModel>(query, continuationToken, queryOptions);
            var result = await iterator.ReadNextAsync();
            return new QueryResult(result.ContinuationToken, result, result.RequestCharge);
        }
    }
}
