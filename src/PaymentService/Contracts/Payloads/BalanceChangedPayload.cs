using PaymentService.Contracts.Enums;

namespace PaymentService.Contracts.Payloads
{
    public class BalanceChangedPayload
    {
        public decimal Amount { get; set; }
        public OperationType OperationType { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
