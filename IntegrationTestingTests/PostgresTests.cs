using System;
using System.Threading.Tasks;
using Ductus.FluentDocker.Services;
using FluentAssertions;
using IntegrationTesting;
using IntegrationTesting.Dependencies;
using IntegrationTesting.Dependencies.Postgres;
using IntegrationTesting.TestServer;
using IntegrationTestingSandbox;
using IntegrationTestingSandbox.DataAccess;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace IntegrationTestingTests
{
    public class PostgresTests : IClassFixture<WebApplicationFactoryBuilder<Startup>>
    {
        private readonly WebApplicationFactory<Startup> _testServer;

        public PostgresTests(WebApplicationFactoryBuilder<Startup> factory)
        {
            _testServer = factory.AddDependenciesBuilder(
                new DependenciesBuilder()
                    .AddDependency(
                        new PostgresDependencyBuilder()
                            .AddConfig(PostgresDependencyConfig.Default)
                            .AddConfigureServices(context =>
                            {
                                var postgresContext = (PostgresRunningDependencyContext) context;
                                context.Services.PostConfigure<PostgresOptions>(options =>
                                    options.ConnectionString = postgresContext.GetConnectionString);
                            })));
        }


        [Fact]
        public async Task InsertToPostgres_Return_ExpectedValue()
        {
            // arrange
            var client = _testServer.CreateClient();
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