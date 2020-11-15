using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ductus.FluentDocker.Services.Extensions;
using IntegrationTesting.DependenciesConfigs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace IntegrationTesting.Dependencies.Postgres
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

        public void Configure(IConfiguration configuration, IServiceCollection services)
        {
            _settingsConfigurator(this, configuration, services);
        }

        public async Task AfterDependencyStart(ServiceProvider services, CancellationToken cancellationToken)
        {
            var connString = GetConnectionString;

            await using var connection = new NpgsqlConnection(connString);
            await connection.OpenAsync(cancellationToken);
            var tablesNames = await GetAllTablesName(connection, cancellationToken);
            await TruncateTables(connection, tablesNames, cancellationToken);
        }

        private static async Task<List<string>> GetAllTablesName(NpgsqlConnection connection,
                                                                 CancellationToken cancellationToken)
        {
            var result = new List<string>();

            await using var cmd = GetAllTablesCommand(connection);
            await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
            while (await reader.ReadAsync(cancellationToken))
                result.Add($"{reader.GetString(0)}.{reader.GetString(1)}");

            return result;
        }

        private static NpgsqlCommand GetAllTablesCommand(NpgsqlConnection connection)
        {
            return new NpgsqlCommand(
                @"SELECT t.table_schema, table_name
                    FROM information_schema.tables as t
                    join (select s.nspname as table_schema,
                    s.oid     as schema_id,
                    u.usename as owner
                    from pg_catalog.pg_namespace s
                        join pg_catalog.pg_user u on u.usesysid = s.nspowner
                    where nspname not in ('information_schema', 'pg_catalog'/*, 'public'*/)
                    and nspname not like 'pg_toast%'
                    and nspname not like 'pg_temp_%'
                    order by table_schema
                        ) as s
                        on s.table_schema = t.table_schema
                    WHERE t.table_type = 'BASE TABLE'"
                , connection);
        }

        private static async Task TruncateTables(NpgsqlConnection connection,
                                                 IEnumerable<string> tablesNames,
                                                 CancellationToken cancellationToken)
        {
            await using var cmd = GetTruncateTablesCommand(tablesNames, connection);
            await cmd.ExecuteNonQueryAsync(cancellationToken);
        }

        private static NpgsqlCommand GetTruncateTablesCommand(IEnumerable<string> tablesNames,
                                                              NpgsqlConnection connection)
        {
            return new NpgsqlCommand($"truncate {string.Join(',', tablesNames)} cascade", connection);
        }
    }
}