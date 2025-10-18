using Microsoft.Extensions.Logging;
using PaymentService.Infrastructures.Contexts;
using Shared.Database.Migrations.Abstracts;

namespace PaymentService.Hosts.Migrations
{
    /// <summary>
    /// Запуск миграций.
    /// </summary>
    internal sealed class Startup
    {
        private readonly IDatabaseMigrationService _databaseMigrationService;
        private readonly ILoggerFactory _loggerFactory;

        public Startup(IDatabaseMigrationService databaseMigrationService, ILoggerFactory loggerFactory)
        {
            _databaseMigrationService = databaseMigrationService ?? throw new ArgumentNullException(nameof(databaseMigrationService));
            _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
        }

        /// <summary>
        /// Выполняет миграции включая применение сидинга.
        /// </summary>
        /// <param name="cancellationToken">Токен отмены.</param>
        /// <exception cref="NotFoundException">Исключение, если не удалось получить контекст базы данных.</exception>
        public async Task StartMigrationsAsync(CancellationToken cancellationToken)
        {
            var runnerLogger = _loggerFactory.CreateLogger<Startup>();

            await _databaseMigrationService.ExecuteMigrationsAsync<PaymentDbContext>(runnerLogger, cancellationToken);
            await _databaseMigrationService.ExecuteSeedingAsync<PaymentDbContext>(runnerLogger, cancellationToken);
        }
    }
}
