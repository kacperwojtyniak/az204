using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using az204functions.Models;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

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
       
    }
}
