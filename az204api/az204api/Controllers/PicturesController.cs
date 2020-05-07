using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using az204api.Models;
using Azure.Identity;
using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using static az204api.Constants;

namespace az204api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PicturesController : ControllerBase
    {
        private readonly BlobServiceClient blobClient;
        private Container container;
        private const string COFFEES_BLOB_CONTAINER = "coffees";
        public PicturesController(CosmosClient cosmosClient, BlobServiceClient blobClient)
        {
            this.container = cosmosClient.GetContainer(DATABASE_ID, COFFEES_CONTAINER);
            this.blobClient = blobClient;
        }

        [HttpPost("{id}/{roastery}")]
        public async Task<IActionResult> Post(string id, string roastery, [FromForm]IFormFile file)
        {
            try
            {
                if (file.ContentType != "image/jpeg")
                {
                    return new UnsupportedMediaTypeResult();
                }
                var coffee = await container.ReadItemAsync<CoffeeModel>(id, new PartitionKey(roastery));
                var fileName = $"{coffee.Resource.Name.Replace(" ", "_").Trim()}.jpg";

                var containerClient = blobClient.GetBlobContainerClient(COFFEES_BLOB_CONTAINER).GetBlobClient(fileName);

                using var stream = file.OpenReadStream();                
                await containerClient.UploadAsync(stream, new BlobHttpHeaders() { ContentType = file.ContentType });
                
                var blobUri = $"{blobClient.Uri}{COFFEES_BLOB_CONTAINER}/{fileName}";
                coffee.Resource.ImgUrl = blobUri;
                await container.UpsertItemAsync(coffee.Resource);

                return Ok(coffee.Resource);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}