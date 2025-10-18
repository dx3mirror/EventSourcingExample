using Shared.Core.Events;

namespace PaymentService.Core.Events
{
    public sealed record WalletCreated(
        Guid OwnerId,
        DateTime CreatedAt // UTC
    ) : IDomainEvent;
}
