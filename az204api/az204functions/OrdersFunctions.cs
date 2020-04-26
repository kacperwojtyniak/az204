using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Documents;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace az204functions
{
	public class OrdersFunctions
	{
		private HttpClient client;

		public OrdersFunctions(IHttpClientFactory httpClientFactory)
		{
			this.client = httpClientFactory.CreateClient("orders-logicapp");
		}
		[FunctionName("ValidateOrder")]
		public async Task ValidateOrder([CosmosDBTrigger(
			databaseName: "%databaseName%",
			collectionName: "%ordersCollection%",
			ConnectionStringSetting = "connectionString",
			CreateLeaseCollectionIfNotExists = true,
			LeaseCollectionName ="leaseOrders")]IReadOnlyList<Document> input, ILogger log)
		{
			throw new NotImplementedException();
		}

		[FunctionName("RequestApproval")]
		public async Task RequestApproval([HttpTrigger(AuthorizationLevel.Anonymous, "post")]HttpRequest req)
		{
			throw new NotImplementedException();
		}

	}
}
