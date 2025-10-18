using PaymentService.Contracts.Payloads;
using PaymentService.Core.Events;
using PaymentService.Core;
using PaymentService.Persistence.Providers.Abstracts;
using System.Text.Json;

namespace PaymentService.Handlers.Commands.WalletCreate
{
    public sealed class WalletCreateCommandHandler
    {
        private readonly IEventStoreProvider _store;
        public WalletCreateCommandHandler(IEventStoreProvider store) => _store = store;

        public async Task<Guid> HandleAsync(WalletCreateCommand cmd, CancellationToken ct = default)
        {
            var nowUtc = DateTime.UtcNow;
            var wallet = new WalletAggregate();
            wallet.CreateNew(cmd.OwnerId, nowUtc);

            var toAppend = wallet.DequeueUncommitted().Select(e => e switch
            {
                WalletCreated wc => (
                    Type: "WalletCreated",
                    PayloadJson: JsonSerializer.Serialize(new WalletCreatedPayload
                    {
                        OwnerId = wc.OwnerId,
                        CreatedAt = wc.CreatedAt
                    }),
                    MetadataJson: (string?)null
                ),
                _ => throw new InvalidOperationException($"Unknown event {e.GetType().Name}")
            }).ToArray();

            await _store.AppendAsync(
                aggregateId: wallet.Id,           // <- сгенерированный Id
                expectedVersion: -1,              // новый стрим
                events: toAppend,
                cancellationToken: ct);

            wallet.MarkCommitted(toAppend.Length);
            return wallet.Id;                     // вернём WalletId вызывающему
        }
    }
}
