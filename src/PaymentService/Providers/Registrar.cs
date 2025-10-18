using Kortros.Common.Infrastructures.DataAccess.Extensions;
using Microsoft.Extensions.DependencyInjection;
using PaymentService.Infrastructures.Common.Configurates;
using PaymentService.Infrastructures.Contexts;
using PaymentService.Persistence.Providers;
using PaymentService.Persistence.Providers.Abstracts;

namespace PaymentService.Providers
{
    /// <summary>
    /// Провайдер по подключениям
    /// </summary>
    public static class Registrar
    {
        public static void AddPostgreDb(this IServiceCollection services)
        {
            services.AddDataAccess<PaymentDbContext, PaymentConfigurates>();
            services.AddScoped<IEventStoreProvider, PaymentEventStoreProvider>();
        }
    }
}
