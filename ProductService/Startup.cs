using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Consul;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ProductService
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
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApplicationLifetime applicationLifetime)
        {

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();

            string ip = Configuration["ip"];
            string port = Configuration["port"];
            string serviceName = "ProductService";
            string serviceId = serviceName + Guid.NewGuid();
            using (var consulClient = new ConsulClient(ConsulConifg))
            {
                consulClient.Agent.ServiceRegister(new AgentServiceRegistration
                {
                    Address = ip,
                    Port = Convert.ToInt32(port),
                    ID = serviceId,
                    Name = serviceName,
                    Check = new AgentServiceCheck
                    {
                        DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(5),
                        HTTP = $"http://{ip}:{port}/api/Health",
                        Interval = TimeSpan.FromSeconds(10),
                        Timeout = TimeSpan.FromSeconds(5)
                    }
                }).Wait();
            }
            applicationLifetime.ApplicationStopped.Register(() =>
            {
                using (var consulClient = new ConsulClient(ConsulConifg))
                {
                    Console.WriteLine("Consul");
                    consulClient.Agent.ServiceDeregister(serviceId).Wait();
                }
            });

        }

        public void ConsulConifg(ConsulClientConfiguration c)
        {
            c.Address = new Uri("http://127.0.0.1:8500");
            c.Datacenter = "dc1";
        }

    }
}
