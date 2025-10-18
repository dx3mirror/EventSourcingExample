using PaymentService.Contracts.Enums;
using Shared.Core.Events;

namespace PaymentService.Core.Events
{
    public sealed record BalanceChanged(
        decimal Amount,
        OperationType OperationType,
        DateTime CreatedAt
    ) : IDomainEvent;
}
