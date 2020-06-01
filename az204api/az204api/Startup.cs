using az204api.Models;
using az204api.Telemetry;
using Azure.Identity;
using Azure.Storage.Blobs;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Text.Json;

namespace az204api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var tempConfig = new Config();
            Configuration.Bind(tempConfig);
            services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
                    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                });
            services.AddSwaggerDocument();
            services.AddApplicationInsightsTelemetry();
            services.AddSingleton<ITelemetryInitializer, TelemetryInitializer>();
            services.AddHealthChecks();
            services.AddSingleton<CosmosClient>(service =>
            {                
                var options = new CosmosClientOptions();
                options.SerializerOptions = new CosmosSerializationOptions() { PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase };
                var client = new CosmosClient(tempConfig.ConnectionString, options);
                return client;
            });
            services.AddTransient(blibClient =>
            {               
                return new BlobServiceClient(new Uri(tempConfig.BlobUrl), new DefaultAzureCredential());
            });

            services.Configure<FormOptions>(options =>
            {
                options.MemoryBufferThreshold = int.MaxValue;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseOpenApi();
            app.UseSwaggerUi3();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/health");
            });
        }
    }
}
