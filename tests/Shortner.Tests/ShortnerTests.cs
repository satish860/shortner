using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using System.Text.Json;
using System.Net.Http.Json;

namespace Shortner.Tests
{
    [Collection("Microservice Integration Tests Fixture")]
    public class ShortnerTests : BaseIntegerationTests
    {
        private readonly HttpClient httpClient;

        public ShortnerTests(IntegrationTestsFixture integrationTestsFixture)
            : base(integrationTestsFixture)
        {
            this.httpClient = integrationTestsFixture.CreateDefaultClient();
        }

        [Fact]
        public async Task Should_Return_ShortUrl_for_Long_url()
        {
            var response = await this.httpClient.PostAsJsonAsync("api/v1/shortner", "https://www.google.co.in/");
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }

        [Fact]
        public async Task Should_Able_Redirect_to_Proper_url()
        {
            var response = await this.httpClient.PostAsJsonAsync("api/v1/shortner", "https://www.google.co.in/");
            var location = response.Headers.Location;
            var redirectUrl = await this.httpClient.GetAsync(location);
            Assert.Equal(HttpStatusCode.MovedPermanently, redirectUrl.StatusCode);
        }
    }
}
