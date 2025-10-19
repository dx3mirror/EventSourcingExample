using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PaymentService.Consumers;
using PaymentService.Contracts;
using PaymentService.Contracts.Events;
using PaymentService.Persistence.Providers.Abstracts;
using Shared.Brokers;
using Shared.Database.Elastic;
using PaymentService.Persistence.Providers;

var builder = Host.CreateApplicationBuilder(args);
const string GroupConsumers = "consumers-log";
builder.Services.AddKafkaConsumers(builder.Configuration, b =>
{
    b.Consumer<WalletCreatedEvent, WalletCreatedEventConsumers>(
        topic: KafkaTopics.WalletCreated,
        group: GroupConsumers);

    b.Consumer<WalletDepositedEvent, WalletDepositedEventConsumer>(
        topic: KafkaTopics.BalanceChanged,
        group: GroupConsumers);
});

builder.Services.AddElasticsearch("http://localhost:9200", enableDebug: true);
builder.Services.AddScoped<IWalletElasticProvider, WalletElasticProvider>();
await builder.Build().RunAsync();
