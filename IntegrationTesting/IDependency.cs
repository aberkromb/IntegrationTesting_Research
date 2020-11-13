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
        ///     Устанавливает необходимые для зависимости настройки
        /// </summary>
        /// <param name="contextConfiguration"></param>
        /// <param name="services"></param>
        void Setup(IConfiguration contextConfiguration, IServiceCollection services);
    }
}