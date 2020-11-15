using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IntegrationTesting
{
    /// <summary>
    ///     Маркерный интерфейс для зависимостей
    /// </summary>
    public interface IDependency
    {
        /// <summary>
        ///     Устанавливает необходимые настройки после запуска зависимости
        /// </summary>
        /// <param name="contextConfiguration"></param>
        /// <param name="services"></param>
        void Configure(IConfiguration contextConfiguration, IServiceCollection services);

        /// <summary>
        ///     Выполняется после того как зависимость запустилась
        /// </summary>
        /// <param name="serviceProvider"></param>
        Task AfterDependencyStart(ServiceProvider services, CancellationToken cancellationToken);
    }
}