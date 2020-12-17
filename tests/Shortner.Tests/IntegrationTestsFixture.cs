using System;
using System.Net.Http;
using Ductus.FluentDocker.Services;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace Shortner.Tests
{
    [CollectionDefinition("Microservice Integration Tests Fixture")]
    public class IntegrationTestsFixture: IDisposable,ICollectionFixture<IntegrationTestsFixture>
    {
        private readonly IConfiguration configuration;

        private readonly IFixture fixture;

        private readonly IContainerService container;

        public IntegrationTestsFixture()
        {
            this.configuration = new ConfigurationBuilder()
                 .AddJsonFile("appsetings.json", true, true)
                 .AddEnvironmentVariables()
                 .Build();

            this.container = new Ductus.FluentDocker.Builders.Builder()
                .UseContainer()
                .ReuseIfExists()
                .WithName("TestContainer")
                .UseImage("redis:alpine")
                .ExposePort(6379, 6379)
                .WaitForPort("6379/tcp",3000)
                .Build().Start();
            this.container.RemoveOnDispose = true;
            this.fixture = new TestFixture(configuration);
        }

        public HttpClient CreateDefaultClient()
        {
            return this.fixture.httpClient;
        }

        public void Dispose()
        {
            this.fixture?.Dispose();
            this.container.Remove(true);
        }
    }
}
