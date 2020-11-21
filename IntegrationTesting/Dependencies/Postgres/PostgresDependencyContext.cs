using System;
using Ductus.FluentDocker.Services;
using Ductus.FluentDocker.Services.Extensions;
using IntegrationTesting.Dependencies.Postgres;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IntegrationTesting.Dependencies
{
    public class PostgresRunningDependencyContext : IRunningDependencyContext
    {
        private readonly PostgresDependencyConfig _config;
        private readonly IContainerService _container;
        private readonly IConfiguration _configuration;
        private readonly IServiceCollection _services;

        public PostgresRunningDependencyContext(PostgresDependencyConfig config,
                                                IContainerService container,
                                                IConfiguration configuration,
                                                IServiceCollection services)
        {
            _config = config;
            _container = container;
            _configuration = configuration;
            _services = services;
        }

        public IConfiguration Configuration => _configuration;

        public IServiceCollection Services => _services;

        public IDependencyConfig DependencyConfig => _config;

        /// <summary>
        ///     Отдает строку подключения для postgres
        /// </summary>
        public string GetConnectionString =>
            $"Host={GetHost()}; Port={_config.ExposePort}; Database={_config.Database}; Username={_config.UserName}; Password={_config.Password}";

        private string GetHost() =>
            _container.ToHostExposedEndpoint($"{_config.ExposePort}/tcp").Address.ToString();
    }
}