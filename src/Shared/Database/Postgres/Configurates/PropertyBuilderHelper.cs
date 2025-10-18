using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Shared.Database.Postgres.Configurates
{
    /// <summary>
    /// Helper для работы с builder'ом свойств.
    /// </summary>
    public static class PropertyBuilderHelper
    {
        /// <summary>
        /// Помечает поле как временную метку.
        /// </summary>
        /// <param name="builder">Builder.</param>
        /// <returns>Builder.</returns>
        public static PropertyBuilder<DateTime> IsTimestamp(this PropertyBuilder<DateTime> builder)
        {
            return builder.HasDefaultValueSql("now()");
        }

        /// <summary>
        /// Помечает поле как временную метку без установки значения по умолчанию.
        /// </summary>
        /// <param name="builder">Builder.</param>
        /// <returns>Builder.</returns>
        public static PropertyBuilder<DateTime> IsTimestampWithoutDefault(this PropertyBuilder<DateTime> builder)
        {
            return builder;
        }

        /// <summary>
        /// Помечает поле как временную метку без установки значения по умолчанию.
        /// </summary>
        /// <param name="builder">Builder.</param>
        /// <returns>Builder.</returns>
        public static PropertyBuilder<DateTime?> IsTimestampWithoutDefault(this PropertyBuilder<DateTime?> builder)
        {
            return builder;
        }

        /// <summary>
        /// Помечает поле как JSON.
        /// </summary>
        /// <param name="builder">Builder.</param>
        /// <returns>Builder.</returns>
        public static PropertyBuilder<string?> IsJson(this PropertyBuilder<string?> builder)
        {
            return builder.HasColumnType("json");
        }

        /// <summary>
        /// Помечает поле как JSON.
        /// </summary>
        /// <param name="builder">Builder.</param>
        /// <returns>Builder.</returns>
        public static PropertyBuilder<string> IsJsonNonNullable(this PropertyBuilder<string> builder)
        {
            return builder.HasColumnType("json");
        }

        /// <summary>
        /// Помечает поле как Text.
        /// </summary>
        /// <param name="builder">Builder.</param>
        /// <returns>Builder.</returns>
        public static PropertyBuilder<string?> IsText(this PropertyBuilder<string?> builder)
        {
            return builder.HasColumnType("text");
        }

        /// <summary>
        /// Помечает поле как Text.
        /// </summary>
        /// <param name="builder">Builder.</param>
        /// <returns>Builder.</returns>
        public static PropertyBuilder<string> IsTextNonNullable(this PropertyBuilder<string> builder)
        {
            return builder.HasColumnType("text");
        }

        /// <summary>
        /// Помечает поле как decimal.
        /// </summary>
        /// <param name="builder">Builder.</param>
        /// <returns>Builder.</returns>
        public static PropertyBuilder<decimal?> IsDecimal(this PropertyBuilder<decimal?> builder)
        {
            return builder.HasColumnType("decimal(18,2)");
        }

        /// <summary>
        /// Помечает поле как decimal.
        /// </summary>
        /// <param name="builder">Builder.</param>
        /// <returns>Builder.</returns>
        public static PropertyBuilder<decimal> IsDecimal(this PropertyBuilder<decimal> builder)
        {
            return builder.HasColumnType("decimal(18,2)");
        }

        /// <summary>
        /// Помечает поле как Guid.
        /// </summary>
        /// <param name="builder">Builder.</param>
        /// <returns>Builder.</returns>
        public static PropertyBuilder<Guid?> IsGuid(this PropertyBuilder<Guid?> builder)
        {
            return builder.HasColumnType("uuid");
        }

        /// <summary>
        /// Помечает поле как Guid.
        /// </summary>
        /// <param name="builder">Builder.</param>
        /// <returns>Builder.</returns>
        public static PropertyBuilder<Guid> IsGuid(this PropertyBuilder<Guid> builder)
        {
            return builder.HasColumnType("uuid").IsRequired();
        }

        /// <summary>
        /// Помечает поле как bool.
        /// </summary>
        /// <param name="builder">Builder.</param>
        /// <returns>Builder.</returns>
        public static PropertyBuilder<bool> IsBoolean(this PropertyBuilder<bool> builder)
        {
            return builder.HasColumnType("boolean").IsRequired();
        }

        /// <summary>
        /// Помечает поле как bool.
        /// </summary>
        /// <param name="builder">Builder.</param>
        /// <returns>Builder.</returns>
        public static PropertyBuilder<bool?> IsBoolean(this PropertyBuilder<bool?> builder)
        {
            return builder.HasColumnType("boolean");
        }

        /// <summary>
        /// Помечает поле как integer.
        /// </summary>
        /// <param name="builder">Builder.</param>
        /// <returns>Builder.</returns>
        public static PropertyBuilder<int?> IsInteger(this PropertyBuilder<int?> builder)
        {
            return builder.HasColumnType("integer");
        }

        /// <summary>
        /// Помечает поле как integer.
        /// </summary>
        /// <param name="builder">Builder.</param>
        /// <returns>Builder.</returns>
        public static PropertyBuilder<int> IsInteger(this PropertyBuilder<int> builder)
        {
            return builder.HasColumnType("integer").IsRequired();
        }

        /// <summary>
        /// Помечает поле как smallint.
        /// </summary>
        /// <param name="builder">Builder.</param>
        /// <returns>Builder.</returns>
        public static PropertyBuilder<byte?> IsSmallInt(this PropertyBuilder<byte?> builder)
        {
            return builder.HasColumnType("smallint");
        }

        /// <summary>
        /// Помечает поле как smallint.
        /// </summary>
        /// <param name="builder">Builder.</param>
        /// <returns>Builder.</returns>
        public static PropertyBuilder<byte> IsSmallInt(this PropertyBuilder<byte> builder)
        {
            return builder.HasColumnType("smallint");
        }

        /// <summary>
        /// Помечает поле как Enum.
        /// </summary>
        /// <param name="builder">Builder.</param>
        /// <returns>Builder.</returns>
        public static PropertyBuilder<TEnum> IsEnum<TEnum>(this PropertyBuilder<TEnum> builder) where TEnum : struct, Enum
        {
            return builder.HasColumnType(typeof(TEnum).Name.ToLower());
        }

        /// <summary>
        /// Помечает поле как Enum.
        /// </summary>
        /// <param name="builder">Builder.</param>
        /// <returns>Builder.</returns>
        public static PropertyBuilder<TEnum?> IsEnum<TEnum>(this PropertyBuilder<TEnum?> builder) where TEnum : struct, Enum
        {
            return builder.HasColumnType(typeof(TEnum).Name.ToLower());
        }

        /// <summary>
        /// Помечает поле перечисления как Text.
        /// </summary>
        /// <param name="builder">Builder.</param>
        /// <returns>Builder.</returns>
        public static PropertyBuilder<TEnum> IsTextEnum<TEnum>(this PropertyBuilder<TEnum> builder) where TEnum : struct, Enum
        {
            return builder.HasColumnType("text");
        }

        /// <summary>
        /// Помечает поле перечисления как Text.
        /// </summary>
        /// <param name="builder">Builder.</param>
        /// <returns>Builder.</returns>
        public static PropertyBuilder<TEnum?> IsTextEnum<TEnum>(this PropertyBuilder<TEnum?> builder) where TEnum : struct, Enum
        {
            return builder.HasColumnType("text");
        }

        /// <summary>
        /// Создает индекс для поля.
        /// </summary>
        /// <param name="builder">Builder сущности.</param>
        /// <param name="propertyName">Имя свойства.</param>
        /// <param name="indexName">Имя индекса.</param>
        /// <returns>Builder.</returns>
        public static EntityTypeBuilder<T> CreateIndex<T>(this EntityTypeBuilder<T> builder, string propertyName, string indexName) where T : class
        {
            var indexBuilder = builder.HasIndex(propertyName);
            indexBuilder.HasDatabaseName(indexName);
            return builder;
        }

        /// <summary>
        /// Создает уникальный индекс для поля.
        /// </summary>
        /// <param name="builder">Builder сущности.</param>
        /// <param name="propertyName">Имя свойства.</param>
        /// <param name="indexName">Имя индекса.</param>
        /// <returns>Builder.</returns>
        public static EntityTypeBuilder<T> CreateUniqueIndex<T>(this EntityTypeBuilder<T> builder, string propertyName, string indexName) where T : class
        {
            var indexBuilder = builder.HasIndex(propertyName).IsUnique();
            indexBuilder.HasDatabaseName(indexName);
            return builder;
        }
    }

    /// <summary>
    /// Константы длины свойств.
    /// </summary>
    public readonly struct PropertyLengthConstants
    {
        /// <summary>
        /// Длина свойства в 2 символа.
        /// </summary>
        public static readonly int Length2 = 2;

        /// <summary>
        /// Длина свойства в 3 символа.
        /// </summary>
        public static readonly int Length3 = 3;

        /// <summary>
        /// Длина свойства в 20 символа.
        /// </summary>
        public static readonly int Length20 = 20;

        /// <summary>
        /// Длина свойства в 40 символа.
        /// </summary>
        public static readonly int Length40 = 40;

        /// <summary>
        /// Длина свойства в 60 символа.
        /// </summary>
        public static readonly int Length60 = 60;

        /// <summary>
        /// Длина свойства в 200 символов.
        /// </summary>
        public static readonly int Length200 = 200;

        /// <summary>
        /// Длина свойства в 400 символов.
        /// </summary>
        public static readonly int Length400 = 400;

        /// <summary>
        /// Длина свойства в 2000 символов.
        /// </summary>
        public static readonly int Length2000 = 2000;

        /// <summary>
        /// Длина свойства в 8000 символов.
        /// </summary>
        public static readonly int LengthMax = 8000;
    }
}
