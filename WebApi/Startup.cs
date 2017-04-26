using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Consul;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Hosting.Server.Features;

namespace WebApi
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddMvc();

            services.Add(new ServiceDescriptor(typeof(IConsulClient), typeof(ConsulClient), ServiceLifetime.Scoped));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IApplicationLifetime lifetime)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseMvc();

            var serverAddressFeature = (IServerAddressesFeature)app.ServerFeatures.FirstOrDefault(f => f.Key ==typeof(IServerAddressesFeature)).Value;

            var serverAddress = new Uri(serverAddressFeature.Addresses.First());

			// Register service with consul
			var registration = new AgentServiceRegistration()
			{
                ID = $"WebApi-{serverAddress.Port}",
				Name = "WebApi",
                Address = $"{serverAddress.Scheme}://{serverAddress.Host}",
				Port = serverAddress.Port,
				Tags = new[] { "Flibble", "Wotsit", "Aardvark" }
			};

			var consulClient = app.ApplicationServices
								.GetRequiredService<IConsulClient>();
            
			consulClient.Agent.ServiceDeregister(registration.ID).Wait();
			consulClient.Agent.ServiceRegister(registration).Wait();


			lifetime.ApplicationStopping.Register(() =>
			{
				consulClient.Agent.ServiceDeregister(registration.ID).Wait();
			});
        }
    }
}
