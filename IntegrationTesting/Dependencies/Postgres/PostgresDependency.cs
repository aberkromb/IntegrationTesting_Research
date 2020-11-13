using System;
using System.Linq;
using Ductus.FluentDocker.Services.Extensions;
using IntegrationTesting.DependenciesConfigs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IntegrationTesting.Dependencies
{
    /// <summary>
    ///     Зависимость postgres
    /// </summary>
    public class PostgresDependency : IDependency
    {
        private readonly PostgresDependencyConfig _config;
        private readonly WebApiTestDependenciesBuilder _dependenciesBuilder;
        private readonly Action<PostgresDependency, IConfiguration, IServiceCollection> _settingsConfigurator;

        public PostgresDependency(PostgresDependencyConfig config, WebApiTestDependenciesBuilder dependenciesBuilder,
            Action<PostgresDependency, IConfiguration, IServiceCollection> settingsConfigurator)
        {
            _config = config;
            _dependenciesBuilder = dependenciesBuilder;
            _settingsConfigurator = settingsConfigurator;
        }

        /// <summary>
        ///     Отдает строку подключения для postgres
        /// </summary>
        public string GetConnectionString =>
            $"Host={GetHost()}; Port={_config.ExposePort}; Database={_config.Database}; Username={_config.UserName}; Password={_config.Password}";

        private string GetHost()
        {
            var container = _dependenciesBuilder.GetContainers
                .First(c => c.Name.Equals(_config.DependencyName, StringComparison.OrdinalIgnoreCase));
            return container.ToHostExposedEndpoint($"{_config.ExposePort}/tcp").Address.ToString();
        }

        public void Setup(IConfiguration configuration, IServiceCollection services)
        {
            _settingsConfigurator(this, configuration, services);
        }
    }
}