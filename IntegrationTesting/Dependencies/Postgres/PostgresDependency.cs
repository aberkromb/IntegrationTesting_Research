using System;
using System.Threading.Tasks;

namespace IntegrationTesting.Dependencies.Postgres
{
    /// <summary>
    ///     Зависимость postgres
    /// </summary>
    public class PostgresDependency : IDependency
    {
        public Task Execute() => throw new NotImplementedException();
    }
}