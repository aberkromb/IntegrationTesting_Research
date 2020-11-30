using System.Threading.Tasks;
using FluentAssertions;
using IntegrationTesting;
using IntegrationTesting.Dependencies;
using IntegrationTesting.Dependencies.Http;
using IntegrationTesting.TestServer;
using IntegrationTestingSandbox;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http;
using Xunit;
using static IntegrationTesting.Dependencies.Http.HttpMockDependencyContext;

namespace IntegrationTestingTests
{
    public class HttpMockTests  : IClassFixture<WebApplicationFactoryBuilder<Startup>>
    {
        private readonly WebApplicationFactoryBuilder<Startup> _testServer;
        private IDependencyManager DependencyManager => _testServer.DependencyManager;
        private HttpMockDependency HttpMock => DependencyManager.GetDependency<HttpMockDependency>();

        public HttpMockTests(WebApplicationFactoryBuilder<Startup> factory)
        {
            _testServer = factory.AddDependenciesBuilder(
                new DependenciesBuilder()
                    .AddDependency(
                        new HttpMockDependencyBuilder()
                            .AddConfig(HttpMockDependencyConfig.Default)
                            .AddConfigureServices(context =>
                            {
                                var httpMockContext = (HttpMockDependencyContext) context;
                                var (host, port) = httpMockContext.GetHostAndPort();
                                context.Services.AddSingleton<IHttpMessageHandlerBuilderFilter>(_ => new TurnRequestToMockFilter(host, port));
                            })));
        }

        [Fact] 
        public async Task ApiTestGoogle_MockSearchPath_ReturnExpectedValue()
        {
            // arrange
            var client = _testServer.CreateClient();
            HttpMock.AddGetMock("/search", new { value = "mock result"});
            
            // act
            var response = await client.GetAsync($"apitest/google");
            var result = await response.Content.ReadAsStringAsync();
            
            // assert
            result.Should().BeEquivalentTo("{\"value\": \"mock result\"}");
        }
        
        private class Mock
        {
            
        }
    }
}