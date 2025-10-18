using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Npgsql;
using Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure;
using Shared.Database.Postgres.Configurations;

namespace Shared.Database.Postgres.Configurates
{
    /// <summary>
    /// Базовый класс конфигуратора БД.
    /// </summary>
    public abstract class BaseDbContextConfigurator<TDbContext>(
        IConfiguration configuration,
        ILoggerFactory loggerFactory)
        : IDbContextOptionsConfigurator<TDbContext>
        where TDbContext : DbContext
    {
        /// <summary>
        /// Строка подключения.
        /// </summary>
        protected abstract string ConnectionStringName { get; }

        /// <inheritdoc />
        public void Configure(DbContextOptionsBuilder<TDbContext> optionsBuilder)
        {
            Configure(optionsBuilder, _ => { });
        }

        /// <inheritdoc/>
        public void Configure(
            DbContextOptionsBuilder<TDbContext> optionsBuilder,
            Action<NpgsqlDbContextOptionsBuilder> npgsqlOptionsAction)
        {
            var connectionString = configuration.GetConnectionString(ConnectionStringName)
                ?? Environment.GetEnvironmentVariable("CONNECTION_STRING");
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException(
                    $"Не удалось найти строку подключения '{ConnectionStringName}'");
            }

            var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);
            ConfigureDataSource(dataSourceBuilder);
            var dataSource = dataSourceBuilder.Build();

            optionsBuilder
                .UseLoggerFactory(loggerFactory)
                .UseNpgsql(dataSource, npgsqlOptionsAction);


            optionsBuilder.EnableSensitiveDataLogging();
        }

        /// <summary>
        /// Конфигурация Builder'а источника данных Npgsql.
        /// </summary>
        /// <param name="builder">Builder.</param>
        protected virtual void ConfigureDataSource(NpgsqlDataSourceBuilder builder)
        {
        }
    }
}
