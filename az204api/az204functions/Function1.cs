using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace az204functions
{
    public static class Function1
    {
        [FunctionName("Function1")]
        public static async Task Run([CosmosDBTrigger(
            databaseName: "%databaseName%",
            collectionName: "%inputCollection%",
            ConnectionStringSetting = "connectionString",
            CreateLeaseCollectionIfNotExists = true)]IReadOnlyList<Document> input,
            [CosmosDB(
                databaseName: "%databaseName%",
                collectionName: "%outputCollection%",
                ConnectionStringSetting = "connectionString")]
                DocumentClient client, ILogger log)
        {
            var databaseName = Environment.GetEnvironmentVariable("databaseName");
            var outputCollection = Environment.GetEnvironmentVariable("outputCollection");
            var collectionUri = UriFactory.CreateDocumentCollectionUri(databaseName, outputCollection);
            foreach (var coffee in input)
            {                
                await client.UpsertDocumentAsync(collectionUri, coffee);                
            }
        }
    }
}
