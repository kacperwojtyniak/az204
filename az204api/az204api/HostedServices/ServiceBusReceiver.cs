using az204api.Models;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace az204api.HostedServices
{
    public class ServiceBusReceiver : IHostedService
    {
        private readonly QueueClient client;
        private readonly ILogger<ServiceBusReceiver> logger;

        public ServiceBusReceiver(ILogger<ServiceBusReceiver> logger, IOptions<Config> configuration)
        {

            var connectionStringBuilder = new Microsoft.Azure.ServiceBus.ServiceBusConnectionStringBuilder(configuration.Value.ServiceBusConnectionString);
            connectionStringBuilder.EntityPath = "queue1";

            this.client = new QueueClient(connectionStringBuilder);
            this.logger = logger;
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            this.client.RegisterMessageHandler(async (msg, cts) =>
            {
                this.logger.LogWarning("Message received in hosted service");
                await this.client.CompleteAsync(msg.SystemProperties.LockToken);
            }, ex => { return Task.CompletedTask; });
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
