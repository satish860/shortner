using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Shortner.Api;
using Xunit;

namespace Shortner.Tests
{
    [Collection("Microservice Integration Tests Fixture")]
    public class PingTests : BaseIntegerationTests
    {
        private HttpClient httpClient;

        public PingTests(IntegrationTestsFixture webApplicationFactory)
            :base(webApplicationFactory)
        {
            this.httpClient = webApplicationFactory.CreateDefaultClient();
        }

        [Fact]
        public async Task Test_Ping_Service_to_make_sure_Alive_Status_is_Returned()
        {
            var response = await this.httpClient.GetAsync("api/v1/ping");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
