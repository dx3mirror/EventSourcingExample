using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PaymentService.Infrastructures.Contexts;
using Shared.Database.Postgres.Configurates;

namespace PaymentService.Infrastructures.Common.Configurates
{
    public class PaymentConfigurates(IConfiguration configuration, ILoggerFactory loggerFactory) : BaseDbContextConfigurator<PaymentDbContext>(configuration, loggerFactory)
    {
        /// <inheritdoc/>
        protected override string ConnectionStringName => "PaymentDb";
    }
}
