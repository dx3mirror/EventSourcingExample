using PaymentService.Contracts.Payloads;
using PaymentService.Core.Events;
using PaymentService.Core;
using PaymentService.Persistence.Providers.Abstracts;
using System.Text.Json;
using MassTransit;
using PaymentService.Contracts.Events;

namespace PaymentService.Handlers.Commands.WalletCreate
{
    public sealed class WalletCreateCommandHandler
    {
        private readonly IEventStoreProvider _store;
        private readonly ITopicProducer<WalletCreatedEvent> _topic;

        public WalletCreateCommandHandler(IEventStoreProvider store, ITopicProducer<WalletCreatedEvent> topic)
        {
            _store = store;
            _topic = topic;
        }

        public async Task<Guid> HandleAsync(WalletCreateCommand cmd, CancellationToken cancellationToken)
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
                aggregateId: wallet.Id,          
                expectedVersion: -1,            
                events: toAppend,
                cancellationToken: cancellationToken);

            wallet.MarkCommitted(toAppend.Length);
            await SendAsync(wallet.Id, wallet.OwnerId, nowUtc, wallet.Balance, cancellationToken);
            return wallet.Id;    
        }

        public async Task SendAsync(Guid aggregateId, Guid ownerId, DateTime createdAt, decimal? balance, CancellationToken cancellationToken)
        {
            await _topic.Produce(new WalletCreatedEvent
            {
                AggregateId = aggregateId,
                OwnerId = ownerId,
                CreatedAt = createdAt,
                Balance = balance
            }, cancellationToken);
        }
    }
}
