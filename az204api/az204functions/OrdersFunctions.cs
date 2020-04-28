using az204functions.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace az204functions
{
	public class OrdersFunctions
	{
		private HttpClient httpClient;

		public OrdersFunctions(IHttpClientFactory httpClientFactory)
		{
			this.httpClient = httpClientFactory.CreateClient("orders-logicapp");
		}
		//[FunctionName("ValidateOrder")]
		//public async Task ValidateOrder([CosmosDBTrigger(
		//	databaseName: "%databaseName%",
		//	collectionName: "%ordersCollection%",
		//	ConnectionStringSetting = "connectionString",
		//	CreateLeaseCollectionIfNotExists = true,
		//	LeaseCollectionName ="leaseOrders")]IReadOnlyList<Document> input, ILogger log)
		//{
		//	//Call logic app
		//	throw new NotImplementedException();
		//}

		[FunctionName("RequestApproval")]
		public async Task RequestApproval([HttpTrigger(AuthorizationLevel.Anonymous, "post")]HttpRequest req, [DurableClient] IDurableClient starter)
		{
			var orderId = req.Query["id"][0];
			var date = req.Query["date"][0];
			await starter.StartNewAsync<OrderModel>("WaitForOrderApproval", input: new OrderModel (){ Id = orderId, Date = date });

		}

		[FunctionName("WaitForOrderApproval")]
		public async Task WaitForOrderApproval([OrchestrationTrigger]IDurableOrchestrationContext context)
		{
			var input = context.GetInput<OrderModel>();
			await context.CallActivityAsync<OrderModel>("SendApprovalRequest", input); // Send email/sms whatever. Right now just add to table

			using var timeoutCts = new CancellationTokenSource();
			DateTime dueTime = context.CurrentUtcDateTime.AddMinutes(5);
			Task durableTimeout = context.CreateTimer(dueTime, timeoutCts.Token);

			Task<ApprovalResult> approvalEvent = context.WaitForExternalEvent<ApprovalResult>("ApprovalEvent");

			if (approvalEvent == await Task.WhenAny(approvalEvent, durableTimeout))
			{
				timeoutCts.Cancel();
				if (approvalEvent.Result.Approved)
				{
					input.Status = "Approved";
					await context.CallActivityAsync<OrderModel>("SetStatus", input);
					return;
				};
			}
			input.Status = "Rejected";
			await context.CallActivityAsync<OrderModel>("SetStatus", input);

		}
		
		[FunctionName("SendApprovalRequest")]
		public void SendApprovalRequest([ActivityTrigger] IDurableActivityContext helloContext, [CosmosDB(
				databaseName: "%databaseName%",
				collectionName: "%ordersCollection%",
				ConnectionStringSetting = "connectionString")]out dynamic document)
		{
			// Should send email/sms/some notification. Now just adding a document to collecion
			var orderId = helloContext.GetInput<OrderModel>().Id;
			var hostname = Environment.GetEnvironmentVariable("WEBSITE_HOSTNAME");
			var url = $"http://{hostname}/runtime/webhooks/durabletask/instances/{helloContext.InstanceId}/raiseEvent/ApprovalEvent";
			document = new { date = DateTime.UtcNow.ToShortDateString(), url = url, type = "Approval", status = "pending", orderId = orderId };
		}

		[FunctionName("SetStatus")]
		public async Task SetStatus([ActivityTrigger]IDurableActivityContext context, [CosmosDB(
				databaseName: "%databaseName%",
				collectionName: "%ordersCollection%",
				ConnectionStringSetting = "connectionString")] DocumentClient client)
		{
			var input = context.GetInput<OrderModel>();
			var databaseName = Environment.GetEnvironmentVariable("databaseName");
			var collection = Environment.GetEnvironmentVariable("ordersCollection");
			var procedure = UriFactory.CreateStoredProcedureUri(databaseName, collection, "setOrderStatus");
			await client.ExecuteStoredProcedureAsync<dynamic>(procedure,new RequestOptions() { PartitionKey = new PartitionKey(input.Date) }, input.Id, "Approved");
		}
	}
}
