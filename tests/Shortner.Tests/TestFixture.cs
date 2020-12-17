using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Shortner.Api;

namespace Shortner.Tests
{
    public class TestFixture : IFixture
    {
        private readonly TestServer testServer;

        private readonly IConfiguration configuration;



        public TestFixture(IConfiguration configuration)
        {
            this.configuration = configuration;
            this.testServer = new TestServer(
                new WebHostBuilder()
                    .UseStartup<Startup>()
                    .UseConfiguration(this.configuration));
        }

        public HttpClient httpClient => this.testServer.CreateClient();

        public void Dispose()
        {
            this.httpClient?.Dispose();
            this.testServer?.Dispose();
        }
    }
}
