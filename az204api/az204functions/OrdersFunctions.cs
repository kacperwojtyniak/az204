using az204functions.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace az204functions
{
    public class OrdersFunctions
    {
        private Config config;
        private HttpClient httpClient;

        public OrdersFunctions(IHttpClientFactory httpClientFactory, IOptionsMonitor<Config> configuration)
        {
            this.config = configuration.CurrentValue;
            this.httpClient = httpClientFactory.CreateClient("orders-logicapp");
        }

        [FunctionName("ValidateOrder")]
        public async Task ValidateOrder([CosmosDBTrigger(
            databaseName: "%databaseName%",
            collectionName: "%ordersCollection%",
            ConnectionStringSetting = "connectionString",
            CreateLeaseCollectionIfNotExists = true,
            LeaseCollectionName ="leaseOrders")]IReadOnlyList<Document> orders, ILogger log)
        {
            foreach (var document in orders)
            {
                var order = JsonSerializer.Deserialize<OrderModel>(document.ToString(), new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
                if (order.Status == OrderStatus.Pending)
                {
                    var bytes = JsonSerializer.SerializeToUtf8Bytes<OrderModel>(order, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
                    var content = new ByteArrayContent(bytes);
                    content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    await this.httpClient.PostAsync(string.Empty, content);
                }
            }
        }

        [FunctionName(nameof(RequestApproval))]
        public async Task RequestApproval([HttpTrigger(AuthorizationLevel.Anonymous, "post")]HttpRequest req, [DurableClient] IDurableClient starter)
        {
            var orderId = req.Query["id"][0];
            var date = req.Query["date"][0];
            await starter.StartNewAsync<OrderModel>(nameof(WaitForOrderApproval), input: new OrderModel() { Id = orderId, Date = date });
        }

        [FunctionName(nameof(WaitForOrderApproval))]
        public async Task WaitForOrderApproval([OrchestrationTrigger]IDurableOrchestrationContext context)
        {
            var input = context.GetInput<OrderModel>();
            await context.CallActivityAsync(nameof(SendApprovalRequest), input); // Send email/sms whatever. Right now just add to table

            using var timeoutCts = new CancellationTokenSource();
            DateTime dueTime = context.CurrentUtcDateTime.AddMinutes(5);
            Task durableTimeout = context.CreateTimer(dueTime, timeoutCts.Token);

            Task<ApprovalResult> approvalEvent = context.WaitForExternalEvent<ApprovalResult>("ApprovalEvent");

            if (approvalEvent == await Task.WhenAny(approvalEvent, durableTimeout))
            {
                timeoutCts.Cancel();
                if (approvalEvent.Result.Approved)
                {
                    input.Status = OrderStatus.Approved;
                    await context.CallActivityAsync(nameof(SetStatus), input);
                    return;
                };
            }
            input.Status = OrderStatus.Rejected;
            await context.CallActivityAsync(nameof(SetStatus), input);

        }

        [FunctionName(nameof(SendApprovalRequest))]
        public void SendApprovalRequest([ActivityTrigger] IDurableActivityContext helloContext, [CosmosDB(
                databaseName: "%databaseName%",
                collectionName: "%ordersCollection%",
                ConnectionStringSetting = "connectionString")]out ApprovalModel document)
        {
            // Should send email/sms/some notification. Now just adding a document to collecion
            var order = helloContext.GetInput<OrderModel>();
            var hostname = Environment.GetEnvironmentVariable("WEBSITE_HOSTNAME");
            var url = $"http://{hostname}/runtime/webhooks/durabletask/instances/{helloContext.InstanceId}/raiseEvent/ApprovalEvent";
            document = new ApprovalModel() { Date = DateTime.UtcNow.ToShortDateString(), Url = url, OrderId = order.Id };
        }

        [FunctionName(nameof(SetStatus))]
        public async Task SetStatus([ActivityTrigger]IDurableActivityContext context, [CosmosDB(
                databaseName: "%databaseName%",
                collectionName: "%ordersCollection%",
                ConnectionStringSetting = "connectionString")] DocumentClient client)
        {
            var input = context.GetInput<OrderModel>();
            var procedure = UriFactory.CreateStoredProcedureUri(config.DatabaseName, config.OrdersCollection, "setOrderStatus");
            await client.ExecuteStoredProcedureAsync<dynamic>(procedure, new RequestOptions() { PartitionKey = new PartitionKey(input.Date) }, input.Id, input.Status);
        }
    }
}
