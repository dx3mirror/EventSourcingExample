using PaymentService.Persistence.Providers.Abstracts;

namespace PaymentService.Handlers.Querys.OwnerGetBalance
{
    public class OwnerGetBalanceQueryHandler
    {
        private readonly IWalletElasticProvider _walletElasticProvider;

        public OwnerGetBalanceQueryHandler(IWalletElasticProvider walletElasticProvider)
        {
            _walletElasticProvider = walletElasticProvider;
        }

        public async Task<decimal?> HandleAsync(OwnerGetBalanceQuery query, CancellationToken cancellationToken)
        {
            return await _walletElasticProvider.GetBalanceByOwnerIdAsync(query.OwnerId, cancellationToken);
        }
    }
}
