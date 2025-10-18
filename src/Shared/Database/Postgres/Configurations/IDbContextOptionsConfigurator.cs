using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure;

namespace Shared.Database.Postgres.Configurations
{
    /// <summary>
    /// Конфигуратор контекста базы данных.
    /// </summary>
    public interface IDbContextOptionsConfigurator<TContext>
        where TContext : DbContext
    {
        /// <summary>
        /// Настраивает контекст.
        /// </summary>
        /// <param name="optionsBuilder">Builder настройки.</param>
        void Configure(DbContextOptionsBuilder<TContext> optionsBuilder);

        /// <summary>
        /// Настраивает контекст.
        /// </summary>
        /// <param name="optionsBuilder">Builder настройки.</param>
        /// <param name="npgsqlOptionsAction">Дополнительная настройки postgres</param>
        void Configure(
            DbContextOptionsBuilder<TContext> optionsBuilder,
            Action<NpgsqlDbContextOptionsBuilder> npgsqlOptionsAction);

    }
}
