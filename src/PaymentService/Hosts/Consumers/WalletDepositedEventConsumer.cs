using MassTransit;
using PaymentService.Contracts.Events;
using PaymentService.Persistence.Providers.Abstracts;

namespace PaymentService.Consumers
{
    public class WalletDepositedEventConsumer : IConsumer<WalletDepositedEvent>
    {
        private readonly IWalletElasticProvider _walletElasticProvider;
        public WalletDepositedEventConsumer(IWalletElasticProvider walletElasticProvider)
        {
            _walletElasticProvider = walletElasticProvider;
        }

        public async Task Consume(ConsumeContext<WalletDepositedEvent> context)
        {
            await _walletElasticProvider.UpdateBalanceByIdAsync(context.Message.WalletId, context.Message.Balance, context.CancellationToken);
        }
    }
}
