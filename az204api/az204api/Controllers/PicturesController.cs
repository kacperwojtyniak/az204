using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Azure.Identity;
using Azure.Storage;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace az204api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PicturesController : ControllerBase
    {
        [HttpPost("{id}")]
        public async Task<IActionResult> Post(string id, [FromForm]IFormFile file)
        {
            try
            {
                var containerEndpoint = "https://az204coffee.blob.core.windows.net/";
                var client = new BlobServiceClient(new Uri(containerEndpoint), new DefaultAzureCredential());                

                var containerClient = client.GetBlobContainerClient("coffees");
                using var stream = file.OpenReadStream();
                var response = await containerClient.UploadBlobAsync(file.FileName, stream);                
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}