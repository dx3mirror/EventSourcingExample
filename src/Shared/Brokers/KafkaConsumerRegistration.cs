using Confluent.Kafka;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Shared.Brokers
{
    /// <summary>
    /// Регистрация только консумеров Kafka через MassTransit Rider.
    /// </summary>
    public static class KafkaConsumerRegistration
    {
        public static IServiceCollection AddKafkaConsumers(
            this IServiceCollection services,
            IConfiguration configuration,
            Action<KafkaConsumerBuilder> configure)
        {
            ArgumentNullException.ThrowIfNull(services);
            ArgumentNullException.ThrowIfNull(configuration);
            ArgumentNullException.ThrowIfNull(configure);

            var bootstrap = configuration["Kafka:BootstrapServers"] ?? "localhost:9092";
            var builder = new KafkaConsumerBuilder();
            configure(builder);

            services.AddMassTransit(x =>
            {
                // Регистрируем все типы консумеров в контейнер
                foreach (var c in builder.Consumers)
                {
                    x.AddConsumer(c.ConsumerType);
                }

                x.UsingInMemory((_, __) => { });

                x.AddRider(rider =>
                {
                    // Регистрируем консумеров для райдера
                    foreach (var c in builder.Consumers)
                    {
                        rider.AddConsumer(c.ConsumerType);
                    }

                    rider.UsingKafka((ctx, k) =>
                    {
                        k.Host(bootstrap);

                        // Биндим топики/группы
                        foreach (var c in builder.Consumers)
                        {
                            c.Bind(ctx, k);
                        }
                    });
                });
            });

            return services;
        }
    }

    public sealed class KafkaConsumerBuilder
    {
        internal List<ConsumerRegistration> Consumers { get; } = new();

        /// <summary>
        /// Регистрирует консюмера TConsumer для сообщений TMessage из топика/группы.
        /// </summary>
        public KafkaConsumerBuilder Consumer<TMessage, TConsumer>(
            string topic,
            string group,
            Action<IKafkaTopicReceiveEndpointConfigurator<Ignore, TMessage>>? configure = null)
            where TMessage : class
            where TConsumer : class, IConsumer<TMessage>
        {
            ArgumentException.ThrowIfNullOrEmpty(topic);
            ArgumentException.ThrowIfNullOrEmpty(group);

            Consumers.Add(new ConsumerRegistration(
                typeof(TMessage),
                typeof(TConsumer),
                topic,
                group,
                (ctx, k) =>
                {
                    k.TopicEndpoint<TMessage>(topic, group, e =>
                    {
                        e.UseMessageRetry(r =>
                        {
                            // экспоненциальные интервалы
                            r.Exponential(
                                retryLimit: 5,
                                minInterval: TimeSpan.FromSeconds(1),
                                maxInterval: TimeSpan.FromSeconds(30),
                                intervalDelta: TimeSpan.FromSeconds(5));
                        });

                        e.UseInMemoryOutbox(ctx);           // дедуп/атомичность publish внутри консюмера
                        e.ConcurrentMessageLimit = 8;    // ограничение параллелизма обработчика

                        e.AutoOffsetReset = Confluent.Kafka.AutoOffsetReset.Earliest;
                        e.CheckpointInterval = TimeSpan.FromSeconds(5); // как часто коммитим оффсеты
                        e.CheckpointMessageCount = 100;                 // или по количеству сообщений

                        e.ConfigureConsumer<TConsumer>(ctx);
                        configure?.Invoke(e);
                    });
                }));

            return this;
        }
    }

    internal sealed class ConsumerRegistration
    {
        public ConsumerRegistration(
            Type messageType,
            Type consumerType,
            string topic,
            string group,
            Action<IRegistrationContext, IKafkaFactoryConfigurator> bind)
        {
            MessageType = messageType;
            ConsumerType = consumerType;
            Topic = topic;
            Group = group;
            Bind = bind;
        }

        public Type MessageType { get; }
        public Type ConsumerType { get; }
        public string Topic { get; }
        public string Group { get; }
        public Action<IRegistrationContext, IKafkaFactoryConfigurator> Bind { get; }
    }
}
