namespace PaymentService.Contracts.Events
{
    public class WalletCreatedEvent
    {
        public Guid AggregateId { get; set; }

        public Guid OwnerId { get; set; }

        public DateTime CreatedAt { get; set; }

        public decimal? Balance { get; set; }
    }
}
