using System.Collections.Generic;

namespace IntegrationTesting.DependenciesConfigs
{
    /// <summary>
    ///     Конфигурирование зависимости postgres
    /// </summary>
    public class PostgresDependencyConfig
    {
        public static PostgresDependencyConfig Default = new PostgresDependencyConfig();

        /// <summary>
        ///     Имя образа
        /// </summary>
        public string Image { get; set; } = "postgres";

        /// <summary>
        ///     Дополнительные переменные окружения если необходимо
        /// </summary>
        public IReadOnlyCollection<(string Key, string Value)> EnvironmentVariables { get; set; } =
            new List<(string Key, string Value)>();

        /// <summary>
        ///     Открыть порты
        /// </summary>
        public uint ExposePort { get; set; } = 5432;

        /// <summary>
        ///     Имя базы данных
        /// </summary>
        public string Database { get; set; } = "postgres";

        /// <summary>
        ///     Имя пользователя
        /// </summary>
        public string UserName { get; set; } = "postgres";

        /// <summary>
        ///     Пароль для доступа к БД
        /// </summary>
        public string Password { get; set; } = "mystrongpassword";

        /// <summary>
        ///     Имя зависимости
        /// </summary>
        public string DependencyName { get; set; } = "postgres-integration-test";

        /// <summary>
        ///     Переиспользовать если такая зависимость уже существует
        /// </summary>
        public bool ReuseDependencyIfExist { get; set; } = true;
    }
}