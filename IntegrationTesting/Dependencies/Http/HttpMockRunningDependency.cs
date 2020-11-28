using System;
using System.Threading;
using System.Threading.Tasks;
using Ductus.FluentDocker.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IntegrationTesting.Dependencies.Http
{
    public class HttpMockRunningDependency : IRunningDependency
    {
        private readonly Action<IRunningDependencyContext> _configureServices;
        private readonly IContainerService _container;
        private readonly HttpMockDependencyConfig _config;
        private HttpMockDependencyContext _context;

        public HttpMockRunningDependency(Action<IRunningDependencyContext> configureServices,
                                         IContainerService container,
                                         HttpMockDependencyConfig config)
        {
            _configureServices = configureServices;
            _container = container;
            _config = config;
        }

        public void ConfigureService(IConfiguration configuration, IServiceCollection services)
        {
            _context = new HttpMockDependencyContext(_config, _container, configuration, services);
            _configureServices(_context);
        }

        public Task<IDependency> AfterDependencyStart(CancellationToken cancellationToken)
        {
            return Task.FromResult<IDependency>(new HttpMockDependency(_context));
        }

        public void Dispose()
        {
            _container?.Dispose();
        }
    }
}