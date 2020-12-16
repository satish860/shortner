using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Shortner.Core;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
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
            var redisIp = configuration.GetValue<string>("Redis:Url");
            var redisPort = configuration.GetValue<int?>("Redis:Port");
            var port = redisPort ?? 6379;
            ConnectionMultiplexer redis = ConnectionMultiplexer.Connect($"{redisIp}:{redisPort}");
            services.AddTransient<IDatabase>((services) =>
            {
               return redis.GetDatabase();
            });
            services.AddTransient<IUniqueIdGenerator, UniqueIdGenerator>();
            services.AddTransient<IUrlRepository, UrlRepository>();
            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env,IUrlRepository urlRepository)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/{token}", async context =>
                 {
                     var token = context.Request.RouteValues["token"];
                     var Id = Base62Convertor.Decode(token.ToString());
                     var url = await urlRepository.GetUrl(Id);
                     context.Response.Redirect(new Uri(url).AbsoluteUri,true);
                     return;
                 });
                endpoints.MapDefaultControllerRoute();
            });
        }
    }
}
