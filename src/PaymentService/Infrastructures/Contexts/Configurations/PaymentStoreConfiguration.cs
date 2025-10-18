using Microsoft.EntityFrameworkCore;
using PaymentService.Infrastructures.Contexts.StoreEntity;
using Shared.Database.Postgres.Configurates;

namespace PaymentService.Infrastructures.Contexts.Configurations
{
    public sealed class PaymentStoreConfiguration : IEntityTypeConfiguration<PaymentStore>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<PaymentStore> builder)
        {
            builder.ToTable("PaymentStore");

            // Композитный ключ по стриму и версии — гарантирует порядок и уникальность в стриме
            builder.HasKey(x => new { x.AggregateId, x.Version });

            // Уникальность EventId — защита от повторной записи одного и того же события
            builder.HasIndex(x => x.EventId).IsUnique();

            // Полезный индекс на выборку по стриму (для ORDER BY Version)
            builder.HasIndex(x => new { x.AggregateId, x.Version });

            // Тип события
            builder.Property(x => x.Type)
                .IsRequired()
                .IsTextNonNullable()
                .HasMaxLength(200);

            // Тело события/метаданные — храним как jsonb в PostgreSQL (если используешь Npgsql)
            builder.Property(x => x.Payload)
                .IsRequired()
                .IsJsonNonNullable();

            builder.Property(x => x.Metadata)
                .IsRequired()
                .HasDefaultValue("{}")
                 .IsJsonNonNullable();

            // Время создания — по умолчанию now() AT TIME ZONE 'UTC'
            builder.Property(x => x.CreatedAt)
                .IsRequired()
                .IsTimestamp();

            // EventId — обязателен, генерим на стороне приложения
            builder.Property(x => x.EventId)
                .IsRequired();

            // (Опционально) ограничение, чтобы Version не был отрицательным
            builder.HasCheckConstraint("CK_PaymentStore_Version_NonNegative", "\"Version\" >= 0");
        }
    }
}
