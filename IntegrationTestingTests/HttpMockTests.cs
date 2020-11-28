using System.Threading.Tasks;
using FluentAssertions;
using IntegrationTesting;
using IntegrationTesting.Dependencies;
using IntegrationTesting.Dependencies.Http;
using IntegrationTesting.TestServer;
using IntegrationTestingSandbox;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using static IntegrationTesting.Dependencies.Http.HttpMockDependencyContext;

namespace IntegrationTestingTests
{
    public class HttpMockTests  : IClassFixture<WebApplicationFactoryBuilder<Startup>>
    {
        private readonly WebApplicationFactoryBuilder<Startup> _testServer;
        private IDependencyManager _dependencyManager => _testServer.DependencyManager;

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
                                context.Services.AddTransient(_ => new TurnRequestToMockHandler(host, port));
                                context.Services
                                    .AddHttpClient("google")
                                    .AddHttpMessageHandler<TurnRequestToMockHandler>();
                            })));
        }

        [Fact]
        public async Task T()
        {
            // arrange
            var client = _testServer.CreateClient();
            var httpMock = _dependencyManager.GetDependency<HttpMockDependency>();
            httpMock.AddGetMock("/search", new { value = "mock result"});
            
            // act
            var response = await client.GetAsync($"apitest/google");
            var result = await response.Content.ReadAsStringAsync();
            
            // assert
            result.Should().BeEquivalentTo("{\"value\": \"mock result\"}");
        }
    }
}