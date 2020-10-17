using System.Threading.Tasks;
using IntegrationTestingSandbox;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace IntegrationTestingTests
{
    public class PingTests : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly WebApplicationFactory<Startup> _factory;

        public PingTests(WebApplicationFactory<Startup> factory)
        {
            _factory = factory;
            
            new IntegrationTesting.WebApiTestBase()
                .AddPostgres()
                .Start();
        }


        [Fact]
        public async Task PingTest()
        {
            // arrange
            var client = _factory.CreateClient();

            // act 
            var response = await client.GetAsync("weatherforecast/ping");

            // assert
            response.EnsureSuccessStatusCode();
        }
    }
}