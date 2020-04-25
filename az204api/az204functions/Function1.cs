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
    public class Function1
    {
        [FunctionName("CopyToCoffeesBrewing")]
        public async Task CopyToCoffeesBrewing([CosmosDBTrigger(
            databaseName: "%databaseName%",
            collectionName: "%inputCollection%",
            ConnectionStringSetting = "connectionString",
            CreateLeaseCollectionIfNotExists = true,
            LeaseCollectionName ="leaseBrewing")]IReadOnlyList<Document> input,
            [CosmosDB(
                databaseName: "%databaseName%",
                collectionName: "%outputCollectionBrewing%",
                ConnectionStringSetting = "connectionString")]
                DocumentClient client, ILogger log)
        {
            var outputCollection = Environment.GetEnvironmentVariable("outputCollectionBrewing");
            await Copy(input, client, outputCollection);
        }

        [FunctionName("CopyToCoffeesOrigin")]
        public async Task CopyToCoffeesOrigin([CosmosDBTrigger(
            databaseName: "%databaseName%",
            collectionName: "%inputCollection%",
            ConnectionStringSetting = "connectionString",
            CreateLeaseCollectionIfNotExists = true,
            LeaseCollectionName ="leaseOrigin")]IReadOnlyList<Document> input,
            [CosmosDB(
                databaseName: "%databaseName%",
                collectionName: "%outputCollectionOrigin%",
                ConnectionStringSetting = "connectionString")]
                DocumentClient client, ILogger log)
        {
            var outputCollection = Environment.GetEnvironmentVariable("outputCollectionOrigin");
            await Copy(input, client, outputCollection);
        }

        private async Task Copy(IReadOnlyList<Document> input, DocumentClient client, string outputCollection)
        {
            var databaseName = Environment.GetEnvironmentVariable("databaseName");
            var collectionUri = UriFactory.CreateDocumentCollectionUri(databaseName, outputCollection);
            foreach (var coffee in input)
            {
                await client.UpsertDocumentAsync(collectionUri, coffee);
            }
        }
    }
}
