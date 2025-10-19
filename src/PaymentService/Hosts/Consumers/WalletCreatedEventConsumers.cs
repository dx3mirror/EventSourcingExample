using MassTransit;
using PaymentService.Contracts.Events;
using PaymentService.Persistence.Providers.Abstracts;

namespace PaymentService.Consumers
{
    internal class WalletCreatedEventConsumers : IConsumer<WalletCreatedEvent>
    {
        private readonly IWalletElasticProvider _walletElasticProvider;
        public WalletCreatedEventConsumers(IWalletElasticProvider walletElasticProvider)
        {
            _walletElasticProvider = walletElasticProvider;
        }
        public async Task Consume(ConsumeContext<WalletCreatedEvent> context)
        {
            Console.WriteLine($"FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF");
            await _walletElasticProvider.IndexAsync(
                context.Message.AggregateId,
                context.Message.OwnerId,
                context.Message.CreatedAt,
                context.Message.Balance,
                context.CancellationToken);
        }
    }
}
