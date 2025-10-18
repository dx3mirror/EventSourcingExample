using PaymentService.Contracts.Payloads;
using PaymentService.Core.Events;
using PaymentService.Infrastructures.Contexts.StoreEntity;
using PaymentService.Persistence.Converters.Abstracts;
using Shared.Core.Events;
using System.Text.Json;

namespace PaymentService.Persistence.Converters
{
    public sealed class JsonEventSerializer : IEventSerializer
    {
        public IDomainEvent ToDomain(PaymentStore row) => row.Type switch
        {
            "WalletCreated" => MapWalletCreated(row),
            "BalanceChanged" => MapBalanceChanged(row),
            _ => throw new InvalidOperationException($"Unknown type '{row.Type}'")
        };

        public (string Type, string PayloadJson, string? MetadataJson) ToStored(IDomainEvent e) => e switch
        {
            WalletCreated wc => ("WalletCreated",
                JsonSerializer.Serialize(new WalletCreatedPayload { OwnerId = wc.OwnerId, CreatedAt = EnsureUtc(wc.CreatedAt) }),
                null),

            BalanceChanged bc => ("BalanceChanged",
                JsonSerializer.Serialize(new BalanceChangedPayload { Amount = bc.Amount, OperationType = bc.OperationType, CreatedAt = EnsureUtc(bc.CreatedAt) }),
                null),

            _ => throw new InvalidOperationException($"Unknown event {e.GetType().Name}")
        };

        private static WalletCreated MapWalletCreated(PaymentStore row)
        {
            var p = JsonSerializer.Deserialize<WalletCreatedPayload>(row.Payload)
                    ?? throw new InvalidOperationException("Invalid WalletCreated payload");
            return new WalletCreated(p.OwnerId, EnsureUtc(p.CreatedAt != default ? p.CreatedAt : row.CreatedAt));
        }

        private static BalanceChanged MapBalanceChanged(PaymentStore row)
        {
            var p = JsonSerializer.Deserialize<BalanceChangedPayload>(row.Payload)
                    ?? throw new InvalidOperationException("Invalid BalanceChanged payload");
            return new BalanceChanged(p.Amount, p.OperationType, EnsureUtc(p.CreatedAt != default ? p.CreatedAt : row.CreatedAt));
        }

        private static DateTime EnsureUtc(DateTime dt)
            => dt.Kind == DateTimeKind.Utc ? dt : DateTime.SpecifyKind(dt, DateTimeKind.Utc);
    }
}
