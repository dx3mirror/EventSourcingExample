using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Shared.Database.Migrations.Abstracts
{
    /// <summary>
    /// Интерфейс для выполнения миграций и наполнения базы данных.
    /// </summary>
    public interface IDatabaseMigrationService
    {
        /// <summary>
        /// Выполняет миграции для указанного контекста базы данных.
        /// </summary>
        /// <typeparam name="TContext">Тип контекста базы данных, наследуемый от DbContext.</typeparam>
        /// <param name="runnerLogger">Логгер для записи информации о процессе миграции.</param>
        /// <param name="cancellationToken">Токен отмены для прерывания операции.</param>
        Task ExecuteMigrationsAsync<TContext>(ILogger runnerLogger, CancellationToken cancellationToken) where TContext : DbContext;

        /// <summary>
        /// Выполняет сидинг для указанного контекста базы данных.
        /// </summary>
        /// <typeparam name="TContext">Тип контекста базы данных, наследуемый от DbContext.</typeparam>
        /// <param name="runnerLogger">Логгер для записи информации о процессе сидинга.</param>
        /// <param name="cancellationToken">Токен отмены для прерывания операции.</param>
        Task ExecuteSeedingAsync<TContext>(ILogger runnerLogger, CancellationToken cancellationToken) where TContext : DbContext;
    }
}
