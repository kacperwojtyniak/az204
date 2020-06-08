using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using az204api.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Core;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace az204api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ServiceBusController : ControllerBase
    {
        private readonly QueueClient client;
        private readonly ILogger<ServiceBusController> logger;
        private readonly Config settings;

        public ServiceBusController(QueueClient client, IOptions<Config> optionsSnapshot, ILogger<ServiceBusController> logger)
        {

            this.client = client;
            this.logger = logger;
            this.settings = optionsSnapshot.Value;
        }

        [HttpPost]
        public async Task<IActionResult> SendTestMessage()
        {
            var msg = System.Text.Encoding.UTF8.GetBytes("TestMsg");
            await this.client.SendAsync(new Message(msg));
            return Ok();
        }


        [HttpGet]
        public async Task<IActionResult> Peek()
        {
            var builder = GetConnectionStringBuilder();
            var receiver = new MessageReceiver(builder);
            var message = await receiver.PeekAsync();
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> ReceiveAndComplete()
        {
            var builder = GetConnectionStringBuilder();
            var receiver = new MessageReceiver(builder);
            var message = await receiver.ReceiveAsync();
            await receiver.CompleteAsync(message.SystemProperties.LockToken);
            this.logger.LogWarning("Service bus message received and completed");
            return Ok();
        }
        [HttpGet]
        public async Task<IActionResult> ReceiveDLQAndComplete()
        {
            var builder = GetConnectionStringBuilder();
            builder.EntityPath = "queue3/$DeadLetterQueue";
            var receiver = new MessageReceiver(builder);
            var message = await receiver.ReceiveAsync();
            await receiver.CompleteAsync(message.SystemProperties.LockToken);
            this.logger.LogWarning("Service bus message received and completed");
            return Ok();
        }
        public async Task<IActionResult> ReceiveAndDeadletter()
        {
            var builder = GetConnectionStringBuilder();
            var receiver = new MessageReceiver(builder);
            var message = await receiver.ReceiveAsync();
            await receiver.DeadLetterAsync(message.SystemProperties.LockToken);
            this.logger.LogWarning("Service bus message received and deadlettered");
            return Ok();
        }

        public async Task<IActionResult> ReceiveAndAbandon()
        {
            var builder = GetConnectionStringBuilder();
            var receiver = new MessageReceiver(builder);
            var message = await receiver.ReceiveAsync();
            await receiver.AbandonAsync(message.SystemProperties.LockToken);
            this.logger.LogWarning("Service bus message received and abandoned");
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> Receive()
        {
            var builder = GetConnectionStringBuilder();
            var receiver = new MessageReceiver(builder);
            var message = await receiver.ReceiveAsync();
            this.logger.LogWarning("Service bus message received but not completed");
            return Ok();
        }

        private ServiceBusConnectionStringBuilder GetConnectionStringBuilder()
        {
            var connectionString = this.settings.ServiceBusConnectionString;
            var builder = new ServiceBusConnectionStringBuilder(connectionString);
            builder.EntityPath = "queue3";
            return builder;
        }
    }
}
