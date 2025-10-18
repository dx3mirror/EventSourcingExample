namespace PaymentService.Handlers.Commands.WalletGetBalance
{
    public sealed record WalletGetBalanceQuery(Guid WalletId, Guid OwnerId);
}
