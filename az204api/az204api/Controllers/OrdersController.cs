using az204api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static az204api.Constants;

namespace az204api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly CosmosClient client;

        public OrdersController(CosmosClient client)
        {
            this.client = client;
        }

        [HttpPost]
        public async Task<IActionResult> Order(AddOrderModel addOrderModel)
        {
            var query = $"SELECT c.id, c.unitPrice FROM c WHERE c.id in ('{string.Join("','", addOrderModel.Coffees.Select(x => x.Id))}')";
            var coffeesContainer = this.client.GetContainer(DATABASE_ID, COFFEES_CONTAINER);
            var coffees = await coffeesContainer.GetItemQueryIterator<CoffeeModel>(query).ReadNextAsync();


            var orderItems = addOrderModel.Coffees.Select(x => new OrderItem(coffees.Single(coffee => coffee.Id == x.Id), x.Quantity));
            var order = new OrderModel(orderItems);

            var ordersContainer = this.client.GetContainer(DATABASE_ID, ORDERS_CONTAINER);
            await ordersContainer.CreateItemWithIdAsync(order);

            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> GetOrders(string date, string continuationToken)
        {
            if (!string.IsNullOrEmpty(continuationToken))
            {
                var tokenBytes = Convert.FromBase64String(continuationToken);
                continuationToken = UTF8Encoding.UTF8.GetString(tokenBytes);
            }
            var query = $"SELECT * FROM c WHERE c.date = '{date}'";
            var ordersContainer = this.client.GetContainer(DATABASE_ID, ORDERS_CONTAINER);
            var result = await ordersContainer.GetItemQueryIterator<OrderModel>(query, continuationToken).ReadNextAsync();
            return Ok(new QueryResult(result.ContinuationToken, result, result.RequestCharge));
        }
    }
}