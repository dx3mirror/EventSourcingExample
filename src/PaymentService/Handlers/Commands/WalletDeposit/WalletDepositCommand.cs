namespace PaymentService.Handlers.Commands.WalletDeposit
{
    public sealed record WalletDepositCommand(
        Guid WalletId,        // Id счёта (AggregateId)
        Guid OwnerId,         // владелец счёта (фиксируется в агрегате)
        decimal Amount);  // время операции
}
