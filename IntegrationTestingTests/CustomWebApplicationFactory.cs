using System;
using IntegrationTesting;
using IntegrationTesting.Dependencies;
using IntegrationTestingSandbox.DataAccess;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace IntegrationTestingTests
{
    public class WebApplicationFactoryBuilder<TStartup>
        : WebApplicationFactory<TStartup> where TStartup : class
    {
        private IDependencyManager _dependencyManager;

        public WebApplicationFactoryBuilder<TStartup> AddDependenciesBuilder(
            WebApiTestDependenciesBuilder dependenciesBuilder)
        {
            _dependencyManager = dependenciesBuilder.Start();
            return this;
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) =>
            {
                services.PostConfigure<PostgresOptions>(options =>
                    options.ConnectionString =
                        "Host=127.0.0.200; Port=5432; Database=postgres; Username=postgres; Password=mystrongpassword");
                // foreach (var dependency in _dependencyResolver.GetDependencies())
                // {
                //     dependency.Setup(context.Configuration, services);
                // }
            });
        }
    }
}