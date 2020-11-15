using System;
using System.Linq;
using System.Threading.Tasks;
using Ductus.FluentDocker.Services;
using Ductus.FluentDocker.Services.Extensions;
using FluentAssertions;
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
    public class PostgresTests : IClassFixture<WebApplicationFactoryBuilder<Startup>>
    {
        private readonly WebApplicationFactory<Startup> _factory;
        private readonly ICompositeService Dependencies;

        public PostgresTests(WebApplicationFactoryBuilder<Startup> factory)
        {
            _factory = factory.AddDependenciesBuilder(
                new WebApiTestDependenciesBuilder()
                    .AddPostgres(PostgresDependencyConfig.Default,
                        (dependency, configuration, services) => services.PostConfigure<PostgresOptions>(options =>
                            options.ConnectionString = dependency.GetConnectionString)));
        }


        [Fact]
        public async Task InsertToPostgres_Return_ExpectedValue()
        {
            // arrange
            var client = _factory.CreateClient();
            var insertData = Guid.NewGuid().ToString();
            var expected = insertData;
            
            // act 
            var response = await client.GetAsync($"apitest/postgres?toInsert={insertData}");
            var result = await response.Content.ReadAsStringAsync();
     
            // assert
            result.Should().BeEquivalentTo(expected);
        }
    }
}