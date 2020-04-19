using az204api.Models;
using Azure.Cosmos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace az204api.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class CoffeeController : ControllerBase
	{
		private const string DATABASE_ID = "Az204";
		private const string CONTAINER_ID = "Coffees";
		private readonly CosmosContainer container;
		private readonly ILogger<CoffeeController> _logger;

		public CoffeeController(CosmosClient client, ILogger<CoffeeController> logger)
		{
			this.container = client.GetContainer(DATABASE_ID, CONTAINER_ID);
			_logger = logger;
		}

		[HttpGet("{roastery}")]
		public async Task<QueryResult> Get(string roastery, [FromQuery]string continuationToken = default(string))
		{

			if (!string.IsNullOrEmpty(continuationToken))
			{
				var tokenBytes = Convert.FromBase64String(continuationToken);
				continuationToken = UTF8Encoding.UTF8.GetString(tokenBytes);
			}
			var query = $"SELECT * FROM c WHERE c.roastery = '{roastery}'";
			QueryDefinition queryDefinition = new QueryDefinition(query);

			await foreach (var page in container.GetItemQueryStreamIterator(queryDefinition, continuationToken, new QueryRequestOptions() { MaxItemCount = 1 }))
			{
				page.Headers.TryGetValue("x-ms-continuation", out continuationToken);
				page.Headers.TryGetValue("x-ms-request-charge", out var charge);
				var coffees = await JsonSerializer.DeserializeAsync<QueryResponse<CoffeeModel>>(page.ContentStream, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase,PropertyNameCaseInsensitive = true });
				return new QueryResult(continuationToken, coffees.Documents, charge);
			}
			return null;
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
