using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Shortner.Api.HealthChecks;
using Shortner.Core;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text.Json;
using System.Threading.Tasks;

namespace Shortner.Api
{
    public class Startup
    {
        private readonly IConfiguration configuration;

        public Startup(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddApiVersioning();
            var redisIp = configuration.GetValue<string>("Redis:Url") ?? "localhost";
            var redisPort = configuration.GetValue<int?>("Redis:Port");
            var port = redisPort ?? 6379;
            Console.WriteLine($"Redis Configuration provided by app is {redisIp} with Port {redisPort}");
            services.AddTransient((services) => {
                RedisConnection.Connect(redisIp, port);
                return RedisConnection.GetDatabase();
            });
            services.AddTransient<IUniqueIdGenerator, UniqueIdGenerator>();
            services.AddTransient<IUrlRepository, UrlRepository>();
            services.AddHealthChecks()
                .AddRedis($"{redisIp}:{port}", "Redis", HealthStatus.Unhealthy, new[] { "ready" });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IUrlRepository urlRepository)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints => {
                endpoints.MapHealthChecks("/hc", new HealthCheckOptions {
                    Predicate = check => !check.Tags.Contains("ready"),
                    ResponseWriter = this.ResponseWriter
                });
                endpoints.MapHealthChecks("/rd", new HealthCheckOptions {
                    Predicate = check => check.Tags.Contains("ready"),
                    ResponseWriter = this.ResponseWriter

                });
                endpoints.MapGet("/{token}", async context => {
                    var token = context.Request.RouteValues["token"];
                    var Id = Base62Convertor.Decode(token.ToString());
                    var url = await urlRepository.GetUrl(Id);
                    context.Response.Redirect(new Uri(url).AbsoluteUri, true);
                    return;
                });
                endpoints.MapDefaultControllerRoute();
            });
        }

        private Task ResponseWriter(HttpContext context,HealthReport report)
        {
            context.Response.ContentType = MediaTypeNames.Application.Json;
            var healthCheck = new HealthCheck {
                Name = "ShortnerAPI",
                Duration = report.TotalDuration,
                Status = report.Status.ToString()
            };
            healthCheck.Info = report.Entries.Select(p => new HealthInfo {
                Key = p.Key,
                Description = p.Value.Description,
                Duration = p.Value.Duration,
                Status = p.Value.Status.ToString(),
                Error = p.Value.Exception.ToString()
            }).ToList();
            return context.Response.WriteAsync(JsonSerializer.Serialize(healthCheck));
        }
    }
}
