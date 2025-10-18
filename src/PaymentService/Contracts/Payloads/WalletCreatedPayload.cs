namespace PaymentService.Contracts.Payloads
{
    public sealed class WalletCreatedPayload
    {
        public Guid OwnerId { get; init; }
        public DateTime CreatedAt { get; init; } // UTC
    }
}
