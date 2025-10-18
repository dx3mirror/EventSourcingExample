namespace PaymentService.Contracts.Requests
{
    public sealed class CreateWalletRequest
    {
        public Guid OwnerId { get; init; }
    }
}
