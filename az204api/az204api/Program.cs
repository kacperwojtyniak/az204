using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Az204api.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureKeyVault;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Az204api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
                .ConfigureAppConfiguration((context, config) =>
                {
                    var localConfig = config.Build();                    
                    var vaultUrl = localConfig.GetValue<string>(nameof(Config.KeyVaultUrl));
                    config.AddAzureKeyVault(new AzureKeyVaultConfigurationOptions(vaultUrl) { ReloadInterval = TimeSpan.FromMinutes(5) });
                });
    }
}
