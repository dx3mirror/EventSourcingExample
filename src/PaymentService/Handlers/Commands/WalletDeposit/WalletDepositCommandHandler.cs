using PaymentService.Core;
using PaymentService.Persistence.Converters.Abstracts;
using PaymentService.Persistence.Providers.Abstracts;

namespace PaymentService.Handlers.Commands.WalletDeposit
{
    public class WalletDepositCommandHandler(IEventStoreProvider store, IEventSerializer serializer)
    {
        public async Task HandleAsync(WalletDepositCommand cmd, CancellationToken ct)
        {
            var rows = await store.ReadStreamAsync(cmd.WalletId, ct);

            var wallet = new WalletAggregate();
            wallet.Init(cmd.WalletId, cmd.OwnerId);
            wallet.Replay(rows.Select(serializer.ToDomain));

            wallet.Deposit(cmd.Amount, DateTime.UtcNow);

            var events = wallet.DequeueUncommitted().Select(serializer.ToStored).ToArray();
            if (events.Length == 0) return;

            await store.AppendAsync(cmd.WalletId, wallet.ExpectedVersion, events, ct);
            wallet.MarkCommitted(events.Length);
        }
    }
}
