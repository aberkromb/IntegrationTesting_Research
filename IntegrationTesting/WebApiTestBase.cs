using System;
using Ductus.FluentDocker.Builders;
using Ductus.FluentDocker.Services;

namespace IntegrationTesting
{
    public class WebApiTestBase : IDisposable
    {
        private ICompositeService Container;

        // protected ContainerBuilder Build();

        private Builder Builder;

        public WebApiTestBase()
        {
            Builder = new Builder();
        }

        public void Start()
        {
            Container = Builder.Build().Start();
        }

        public WebApiTestBase AddPostgres()
        {
            Builder
                .UseContainer()
                .UseImage("postgres")
                .WithEnvironment($"POSTGRES_PASSWORD=mysecretpassword")
                .ExposePort(5432, 5432)
                .WaitForPort("5432/tcp", 30000 /*30s*/)
                .WithName("postgres-integration-testing")
                .ReuseIfExists()
                .Builder();

            return this;
        }


        public void Dispose()
        {
            var c = Container;
            Container = null;
            try
            {
                c?.Dispose();
            }
            catch
            {
                // Ignore
            }
        }
    }
}