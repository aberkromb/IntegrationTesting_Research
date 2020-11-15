using System.Threading;
using IntegrationTesting;
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
                foreach (var dependency in _dependencyManager.GetDependencies())
                {
                    dependency.Configure(context.Configuration, services);
                }

                var serviceProvider = services.BuildServiceProvider();

                foreach (var dependency in _dependencyManager.GetDependencies())
                {
                    dependency.AfterDependencyStart(serviceProvider, CancellationToken.None);
                }
            });
        }
    }
}