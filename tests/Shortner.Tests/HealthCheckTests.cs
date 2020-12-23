using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Shortner.Tests
{
    [Collection("Microservice Integration Tests Fixture")]
    public class HealthCheckTests :  BaseIntegerationTests
    {
        private HttpClient httpClient;
        public HealthCheckTests(IntegrationTestsFixture webApplicationFactory)
            :base(webApplicationFactory)
        {
            this.httpClient = webApplicationFactory.CreateDefaultClient();
        }

        [Fact]
        public async Task Should_be_able_to_Get_the_health_Status()
        {
            var response = await this.httpClient.GetAsync("/hc");
            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Should_be_able_to_Get_the_ready_Status()
        {
            var response = await this.httpClient.GetAsync("/rd");
            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
        }
    }
}
