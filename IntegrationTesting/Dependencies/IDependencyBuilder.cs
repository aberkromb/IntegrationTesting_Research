namespace IntegrationTesting.Dependencies.Postgres
{
    /// <summary>
    ///     Интерфейс для создания зависимостей
    /// </summary>
    public interface IDependencyBuilder
    {
        /// <summary>
        ///     Добавить конфиг зависимости
        /// </summary>
        PostgresDependencyBuilder AddConfig(IDependencyConfig dependencyConfig);

        /// <summary>
        ///     Запускает зависимость
        /// </summary>
        IRunningDependency Start();
    }
}