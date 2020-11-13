using System.Linq;
using System.Threading.Tasks;
using Ductus.FluentDocker.Services;
using Ductus.FluentDocker.Services.Extensions;
using IntegrationTesting;
using IntegrationTesting.Dependencies;
using IntegrationTesting.DependenciesConfigs;
using IntegrationTestingSandbox;
using IntegrationTestingSandbox.DataAccess;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace IntegrationTestingTests
{
    public class PingTests : IClassFixture<WebApplicationFactoryBuilder<Startup>>
    {
        private readonly WebApplicationFactory<Startup> _factory;
        private readonly ICompositeService Dependencies;

        public PingTests(WebApplicationFactoryBuilder<Startup> factory)
        {
            _factory = factory.AddDependenciesBuilder(
                new WebApiTestDependenciesBuilder()
                    .AddPostgres(PostgresDependencyConfig.Default, (dependency, configuration, services) => services.PostConfigure<PostgresOptions>(options => options.ConnectionString = dependency.GetConnectionString)));
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