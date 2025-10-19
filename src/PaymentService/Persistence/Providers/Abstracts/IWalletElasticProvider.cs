namespace PaymentService.Persistence.Providers.Abstracts
{
    public interface IWalletElasticProvider
    {
        Task IndexAsync(Guid aggregateId, Guid ownerId, DateTime createdAtUtc, decimal? balance, CancellationToken cancellationToken);

        Task<decimal?> GetBalanceByOwnerIdAsync(Guid ownerId, CancellationToken cancellationToken);

        Task UpdateBalanceByIdAsync(Guid walletId, decimal? newBalance, CancellationToken cancellationToken);
    }
}
