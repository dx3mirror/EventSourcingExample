using Kortros.Common.Infrastructures.DataAccess.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PaymentService.Contracts;
using PaymentService.Contracts.Events;
using PaymentService.Infrastructures.Common.Configurates;
using PaymentService.Infrastructures.Contexts;
using PaymentService.Persistence.Providers;
using PaymentService.Persistence.Providers.Abstracts;
using Shared.Brokers;

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

        public static void AddMassTransitProduceProviders(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMassTransitWithKafkaFromModules(
                configuration,
                new WalletCreatedProducerModule(),
                new WalletDepositedProducerModule());
        }

        public sealed class WalletCreatedProducerModule : IKafkaProducerModule
        {
            public void Register(KafkaProducerBuilder b) =>
                b.Producer<WalletCreatedEvent>(KafkaTopics.WalletCreated);
        }

        public sealed class WalletDepositedProducerModule : IKafkaProducerModule
        {
            public void Register(KafkaProducerBuilder b) =>
                b.Producer<WalletDepositedEvent>(KafkaTopics.BalanceChanged);
        }
    }
}
