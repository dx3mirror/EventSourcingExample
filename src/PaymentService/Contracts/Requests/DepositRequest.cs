namespace PaymentService.Contracts.Requests
{
    public class DepositRequest
    {
        public Guid WalletId { get; set; }
        public Guid OwnerId { get; set; }
        public decimal Amount { get; set; }
    }
}
