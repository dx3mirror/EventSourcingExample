using Microsoft.EntityFrameworkCore;
using PaymentService.Infrastructures.Contexts.Configurations;
using Shared.Database.Postgres.Configurations;

namespace PaymentService.Infrastructures.Contexts
{
    internal class CustomModelBuilder
    {
        /// <summary>
        /// Создает модель.
        /// </summary>
        /// <param name="modelBuilder">Билдер.</param>
        public static void OnModelCreating(ModelBuilder modelBuilder)
        {
            ConfigureForecastModels(modelBuilder);

            modelBuilder.SetDefaultDateTimeKind(DateTimeKind.Utc);
        }

        private static void ConfigureForecastModels(ModelBuilder modelBuilder)
        {
            modelBuilder.HasPostgresExtension("uuid-ossp");
            modelBuilder.ApplyConfiguration(new PaymentStoreConfiguration());
        }
    }
}
