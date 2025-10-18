using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Shared.Database.Migrations.Abstracts;

namespace Shared.Database.Migrations
{
    /// <summary>
    /// Сервис для выполнения миграций и наполнения базы данных.
    /// </summary>
    public class DatabaseMigrationService : IDatabaseMigrationService
    {
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// Конструктор, принимающий провайдер сервисов.
        /// </summary>
        /// <param name="serviceProvider">Провайдер сервисов.</param>
        public DatabaseMigrationService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        /// <summary>
        /// Выполняет миграции для указанного контекста базы данных.
        /// </summary>
        /// <typeparam name="TContext">Тип контекста базы данных, наследуемый от DbContext.</typeparam>
        /// <param name="runnerLogger">Логгер для записи информации о процессе миграции.</param>
        /// <param name="cancellationToken">Токен отмены для прерывания операции.</param>
        public async Task ExecuteMigrationsAsync<TContext>(ILogger runnerLogger, CancellationToken cancellationToken) where TContext : DbContext
        {
            runnerLogger.LogInformation($"{nameof(ExecuteMigrationsAsync)}: Запуск миграции для {typeof(TContext).Name}");

            var dbContext = _serviceProvider.CreateScope().ServiceProvider.GetService<TContext>()
            ?? throw new InvalidCastException($"Не удалось получить контекст доступа к базе данных: {typeof(TContext).Name}");

            try
            {
                runnerLogger.LogInformation($"{nameof(ExecuteMigrationsAsync)}: Попытка подключиться к базе данных");
                await dbContext.Database.MigrateAsync(cancellationToken);
            }
            catch (Exception e)
            {
                runnerLogger.LogError(e, $"{nameof(ExecuteMigrationsAsync)}: Не удалось подключиться к базе данных");
                throw;
            }

            runnerLogger.LogInformation($"{nameof(ExecuteMigrationsAsync)}: Все миграции накачены для {typeof(TContext).Name}");
        }

        /// <summary>
        /// Выполняет сидинг для указанного контекста базы данных.
        /// </summary>
        /// <typeparam name="TContext">Тип контекста базы данных, наследуемый от DbContext.</typeparam>
        /// <param name="runnerLogger">Логгер для записи информации о процессе сидинга.</param>
        /// <param name="cancellationToken">Токен отмены для прерывания операции.</param>
        public async Task ExecuteSeedingAsync<TContext>(ILogger runnerLogger, CancellationToken cancellationToken) where TContext : DbContext
        {
            runnerLogger.LogInformation($"{nameof(ExecuteSeedingAsync)}: Запуск наполнения базы данных для {typeof(TContext).Name}");

            var dbContext = _serviceProvider.CreateScope().ServiceProvider.GetService<TContext>()
            ?? throw new InvalidCastException($"Не удалось получить контекст доступа к базе данных: {typeof(TContext).Name}");

            var contextType = dbContext.GetType();
            var seedExecutorType = typeof(SeedExecutor<>).MakeGenericType(contextType);
            var seedExecutor = Activator.CreateInstance(seedExecutorType, dbContext) as ISeedExecutor
            ?? throw new InvalidOperationException("Не удалось создать экземпляр SeedExecutor");

            try
            {
                runnerLogger.LogInformation($"{nameof(ExecuteSeedingAsync)}: Попытка подключиться к базе данных");
                await seedExecutor.RunAsync(cancellationToken);
            }
            catch (Exception e)
            {
                runnerLogger.LogError(e, $"{nameof(ExecuteSeedingAsync)}: Не удалось подключиться к базе данных");
                throw;
            }

            runnerLogger.LogInformation($"{nameof(ExecuteSeedingAsync)}: Наполнение базы завершено для {typeof(TContext).Name}");
        }
    }
}
