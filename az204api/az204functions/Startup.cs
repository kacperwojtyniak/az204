using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System;

[assembly: FunctionsStartup(typeof(az204functions.Startup))]
namespace az204functions
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddHttpClient("orders-logicapp",x=> x.BaseAddress = new Uri(Environment.GetEnvironmentVariable("ordersLogicappUrl")));
        }
    }
}