using az204api.Models;
using Azure.Cosmos;
using Azure.Cosmos.Serialization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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
			services.AddControllers()
				.AddJsonOptions(options =>
				{
					options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
					options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
				});
			services.AddSwaggerDocument();

			services.AddSingleton<CosmosClient>(service =>
			{
				var conn = Configuration.GetValue<string>(nameof(Config.CosmosDbConnString));
				var options = new CosmosClientOptions();
				options.SerializerOptions = new CosmosSerializationOptions() { PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase };

				return new CosmosClient(conn, options);
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
			});
		}
	}
}
