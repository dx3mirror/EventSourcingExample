namespace PaymentService.Handlers.Querys.WalletGetBalance
{
    public sealed record WalletGetBalanceQuery(Guid WalletId, Guid OwnerId);
}
