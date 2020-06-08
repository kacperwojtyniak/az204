using Az204functions.Models;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

[assembly: FunctionsStartup(typeof(Az204functions.Startup))]
namespace Az204functions
{
    public class Startup : FunctionsStartup
    {
        public IConfiguration Configuration { get; }
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddHttpClient("orders-logicapp",x=> x.BaseAddress = new Uri(Environment.GetEnvironmentVariable("ordersLogicappUrl")));
            builder.Services.AddOptions<Config>().Configure<IConfiguration>((settings,configuration) => configuration.Bind(settings));
        }
    }
}