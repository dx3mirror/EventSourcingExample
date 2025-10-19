namespace PaymentService.Contracts.Events
{
    public class WalletDepositedEvent
    {
        public Guid WalletId { get; set; }

        public decimal Balance { get; set; }
    }
}
