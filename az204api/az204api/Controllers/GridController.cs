using az204api;
using az204api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace AllInOneApi.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class GridController : ControllerBase
    {
        private readonly ILogger<GridController> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _secretKey;

        public GridController(ILogger<GridController> logger, IOptions<Config> configuration, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _secretKey = configuration.Value.SecretKey;
        }


        //Test endpoint to send somethig to EventGrid
        [HttpPost]
        public async Task Post([FromBody] EventGridMessageModel messageModel)
        {
            var @event = new EventGridEvent(
                id: Guid.NewGuid().ToString(),
                subject: Constants.EVENT_GRID_SUBJECT,
                data: messageModel,
                eventType: Constants.EVENT_GRID_TYPE,
                eventTime: DateTime.Now,
                dataVersion: Constants.EVENT_GRID_DATA_VERSION);

            var content = new EventGridEvent[] { @event };
            var stringContent = JsonSerializer.Serialize(content);

            var client = _httpClientFactory.CreateClient(Constants.EVENT_GRID_CLIENT);
            await client.PostAsync(string.Empty, new StringContent(stringContent));

        }


        //Webhook called by EventGrid, register in Azure
        [HttpPost]
        public async Task<IActionResult> Webhook([FromQuery] string secretKey)
        {
            try
            {
                _logger.LogInformation("Webhook triggered");
                if (secretKey != _secretKey)
                {
                    _logger.LogWarning($"Unauthorized key provided {secretKey}");
                    return Unauthorized();
                }
                var eventGridEvents = await JsonSerializer.DeserializeAsync<EventGridEvent[]>(Request.Body);

                _logger.LogInformation($"Event count {eventGridEvents.Length}");

                foreach (var @event in eventGridEvents)
                {
                    if (@event.Data is SubscriptionValidationEventData)
                    {
                        _logger.LogInformation("Subscription validation called.");
                        var eventData = (SubscriptionValidationEventData)@event.Data;

                        var response = new SubscriptionValidationResponse()
                        {
                            ValidationResponse = eventData.ValidationCode
                        };
                        return Ok(response);
                    }
                    else if (@event.EventType == Constants.EVENT_GRID_TYPE)
                    {
                        _logger.LogInformation($"Received {Constants.EVENT_GRID_TYPE} event.");
                        var eventData = ((JsonElement)@event.Data).GetRawText();

                        var model = JsonSerializer.Deserialize<EventGridMessageModel>(eventData);

                        _logger.LogInformation($"Received custom event with message {model.Message}");
                        return Ok();
                    }
                }
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed webhook");
                throw;
            }

        }
    }
}
