using MassTransit;
using PaymentService.Contracts.Events;
using PaymentService.Core;
using PaymentService.Persistence.Converters.Abstracts;
using PaymentService.Persistence.Providers.Abstracts;

namespace PaymentService.Handlers.Commands.WalletDeposit
{
    public class WalletDepositCommandHandler(IEventStoreProvider store, IEventSerializer serializer, ITopicProducer<WalletDepositedEvent> topic)
    {
        private readonly ITopicProducer<WalletDepositedEvent> _topic = topic;
        private readonly IEventStoreProvider _eventStoreProvider = store;

        public async Task HandleAsync(WalletDepositCommand command, CancellationToken cancellationToken)
        {
            var rows = await _eventStoreProvider.ReadStreamAsync(command.WalletId, cancellationToken);

            var wallet = new WalletAggregate();
            wallet.Init(command.WalletId, command.OwnerId);
            wallet.Replay(rows.Select(serializer.ToDomain));

            wallet.Deposit(command.Amount, DateTime.UtcNow);

            var events = wallet.DequeueUncommitted().Select(serializer.ToStored).ToArray();
            if (events.Length == 0) return;

            await _eventStoreProvider.AppendAsync(command.WalletId, wallet.ExpectedVersion, events, cancellationToken);
            wallet.MarkCommitted(events.Length);
            await SendAsync(command.WalletId, wallet.Balance, cancellationToken);
        }

        private async Task SendAsync(Guid walletId, decimal balance, CancellationToken cancellationToken)
        {
            var @event = new WalletDepositedEvent
            {
                WalletId = walletId,
                Balance = balance,
            };

            await _topic.Produce(@event, cancellationToken);
        }
    }
}
