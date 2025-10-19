using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Shared.Brokers
{
    // интерфейс модуля
    public interface IKafkaProducerModule
    {
        void Register(KafkaProducerBuilder builder);
    }

    // билдер продюсеров
    public sealed class KafkaProducerBuilder
    {
        private readonly List<Action<IRiderRegistrationConfigurator>> _actions = new();
        public IReadOnlyList<Action<IRiderRegistrationConfigurator>> Actions => _actions;

        public KafkaProducerBuilder Producer<T>(string topic) where T : class
        {
            _actions.Add(r => r.AddProducer<T>(topic));
            return this;
        }
    }

    public static class MassTransitKafkaRegistration
    {
        // регистрируй модули сколько угодно раз
        public static IServiceCollection AddKafkaProducerModule<TModule>(this IServiceCollection services)
            where TModule : class, IKafkaProducerModule, new()
            => services.AddSingleton<IKafkaProducerModule, TModule>();

        public static IServiceCollection AddMassTransitWithKafkaFromModules(
            this IServiceCollection services,
            IConfiguration configuration,
            params IKafkaProducerModule[] modules)
        {
            var bootstrap = configuration["Kafka:BootstrapServers"] ?? "localhost:9092";

            var builder = new KafkaProducerBuilder();
            foreach (var m in modules)
                m.Register(builder);

            services.AddMassTransit(x =>
            {
                x.SetKebabCaseEndpointNameFormatter();

                x.UsingInMemory((ctx, cfg) =>
                {
                    cfg.ConfigureEndpoints(ctx);
                });

                x.AddRider(rider =>
                {
                    foreach (var action in builder.Actions)
                        action(rider);

                    rider.UsingKafka((ctx, k) =>
                    {
                        k.Host(bootstrap);
                    });
                });
            });

            return services;
        }
    }
}
