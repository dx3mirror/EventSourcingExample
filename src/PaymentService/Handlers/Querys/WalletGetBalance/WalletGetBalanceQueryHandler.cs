using PaymentService.Core;
using PaymentService.Persistence.Converters.Abstracts;
using PaymentService.Persistence.Providers.Abstracts;

namespace PaymentService.Handlers.Querys.WalletGetBalance
{
    public sealed class WalletGetBalanceQueryHandler
    {
        private readonly IEventStoreProvider _store;
        private readonly IEventSerializer _serializer;

        public WalletGetBalanceQueryHandler(IEventStoreProvider store, IEventSerializer serializer)
        {
            _store = store;
            _serializer = serializer;
        }

        public async Task<decimal> HandleAsync(WalletGetBalanceQuery q, CancellationToken ct = default)
        {
            var rows = await _store.ReadStreamAsync(q.WalletId, ct);

            var wallet = new WalletAggregate();
            wallet.Init(q.WalletId, q.OwnerId);
            wallet.Replay(rows.Select(_serializer.ToDomain));

            return wallet.Balance;
        }
    }
}
