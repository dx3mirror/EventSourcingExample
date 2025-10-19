namespace PaymentService.Persistence.Providers.Models
{
    public sealed class WalletDocument
    {
        public string Id { get; set; } = default!;
        public Guid OwnerId { get; set; }
        public DateTime CreatedAt { get; set; }
        public decimal? Balance { get; set; }
    }
}
