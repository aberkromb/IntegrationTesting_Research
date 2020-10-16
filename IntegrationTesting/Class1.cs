using System;
using Ductus.FluentDocker.Builders;
using Ductus.FluentDocker.Services;

namespace IntegrationTesting
{
    public abstract class WebApiTestBase : IDisposable
    {
        protected IContainerService Container;

        protected abstract ContainerBuilder Build();

        protected WebApiTestBase()
        {
            Container = new Builder()
                .UseContainer().ReuseIfExists()
                .UseImage("postgres")
                .WithEnvironment($"POSTGRES_PASSWORD=mysecretpassword ")
                .ExposePort(5432)
                .WaitForPort("5432/tcp", 30000 /*30s*/)
                .Build();

            try
            {
                Container.Start();
            }
            catch
            {
                Container.Dispose();
                throw;
            }
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