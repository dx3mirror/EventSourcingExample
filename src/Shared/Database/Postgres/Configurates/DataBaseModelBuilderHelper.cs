using Microsoft.EntityFrameworkCore;
using Npgsql;
using Npgsql.NameTranslation;

namespace Shared.Database.Postgres.Configurates
{
    /// <summary>
    /// Helper для построителя модели базы данных.
    /// </summary>
    public static class DataBaseModelBuilderHelper
    {
        /// <summary>
        /// Помечает поле как Enum.
        /// </summary>
        /// <typeparam name="TEnum">Тип перечисления.</typeparam>
        /// <param name="modelBuilder">Builder.</param>
        /// <returns>Builder.</returns>
        public static ModelBuilder HasEnumType<TEnum>(this ModelBuilder modelBuilder) where TEnum : struct, Enum
        {
            return modelBuilder.HasPostgresEnum<TEnum>(name: typeof(TEnum).Name.ToLower(), nameTranslator: new NpgsqlNullNameTranslator());
        }

        /// <summary>
        /// Маппинг Enum для БД.
        /// </summary>
        /// <typeparam name="TEnum">Тип перечисления.</typeparam>
        /// <param name="builder">Builder.</param>
        /// <returns>Builder.</returns>
        public static NpgsqlDataSourceBuilder MapEnumType<TEnum>(this NpgsqlDataSourceBuilder builder) where TEnum : struct, Enum
        {
            builder.MapEnum<TEnum>(typeof(TEnum).Name.ToLower(), nameTranslator: new NpgsqlNullNameTranslator());
            return builder;
        }
    }
}
