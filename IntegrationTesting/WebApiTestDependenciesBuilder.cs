using System;
using System.Collections.Generic;
using System.Linq;
using Ductus.FluentDocker.Builders;
using Ductus.FluentDocker.Services;
using IntegrationTesting.Dependencies;
using IntegrationTesting.Dependencies.Postgres;
using IntegrationTesting.DependenciesConfigs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IntegrationTesting
{
    public class WebApiTestDependenciesBuilder : IDisposable
    {
        private ICompositeService _compositeService;

        private readonly List<IDependency> _dependencies;

        private readonly Builder _builder;

        public WebApiTestDependenciesBuilder()
        {
            _builder = new Builder();
            _dependencies = new List<IDependency>();
        }

        public IReadOnlyCollection<IContainerService> GetContainers => _compositeService.Containers;

        public IDependencyManager Start()
        {
            _compositeService = _builder.Build().Start();

            return new DependencyManager(_dependencies);
        }

        public WebApiTestDependenciesBuilder AddPostgres(PostgresDependencyConfig config,
            Action<PostgresDependency, IConfiguration, IServiceCollection> configureSettings = null)
        {
            var builder = _builder
                .UseContainer()
                .UseImage(config.Image)
                .WithEnvironment(
                    config.EnvironmentVariables
                        .Select(s => $"{s.Key}={s.Value}")
                        .Concat(new[]
                        {
                            $"POSTGRES_PASSWORD={config.Password}",
                            $"POSTGRES_DB={config.Database}",
                            $"POSTGRES_USER={config.UserName}",
                        })
                        .ToArray())
                .ExposePort((int) config.ExposePort, (int) config.ExposePort)
                .WaitForPort($"{config.ExposePort.ToString()}/tcp", 30000 /*30s*/)
                .WithName(config.DependencyName);

            if (config.ReuseDependencyIfExist)
                builder.ReuseIfExists();

            _dependencies.Add(new PostgresDependency(config, this, configureSettings));

            return this;
        }


        public void Dispose()
        {
            var c = _compositeService;
            _compositeService = null;
            try
            {
                c?.Dispose();
            }
            catch
            {
                // ignore
            }
        }
    }
}