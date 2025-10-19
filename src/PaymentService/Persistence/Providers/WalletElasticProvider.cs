using Elastic.Clients.Elasticsearch;
using PaymentService.Persistence.Providers.Abstracts;
using PaymentService.Persistence.Providers.Models;
using Shared.Database.Elastic.Abstracts;

namespace PaymentService.Persistence.Providers
{
    public class WalletElasticProvider : IWalletElasticProvider
    {
        private readonly IElasticRepository<WalletDocument> _repository;

        public WalletElasticProvider(IElasticRepository<WalletDocument> repository)
        {
            ArgumentNullException.ThrowIfNull(repository);
            _repository = repository;
        }

        public async Task IndexAsync(
            Guid aggregateId, Guid ownerId, DateTime createdAtUtc, decimal? balance, CancellationToken cancellationToken)
        {
            var doc = new WalletDocument
            {
                Id = aggregateId.ToString(),
                OwnerId = ownerId,
                CreatedAt = createdAtUtc,
                Balance = balance
            };

            await _repository.IndexAsync(doc.Id, doc, cancellationToken);
        }

        public async Task<decimal?> GetBalanceByOwnerIdAsync(Guid ownerId, CancellationToken cancellationToken)
        {
            var searchResponse = await _repository.SearchAsync(searchDescriptor =>
            {
                searchDescriptor.Size(1);

                _ = searchDescriptor.Query(queryDescriptor =>
                    queryDescriptor.Term(termQuery =>
                        termQuery.Field("ownerId.keyword")
                                 .Value(ownerId.ToString())));

                searchDescriptor.Sort(sortDescriptor =>
                    sortDescriptor.Field(field => field.CreatedAt,
                        fieldSort => fieldSort.Order(SortOrder.Desc)));
            }, cancellationToken);

            var document = searchResponse.Documents.FirstOrDefault();
            return document?.Balance;
        }

        public async Task UpdateBalanceByIdAsync(Guid walletId, decimal? newBalance, CancellationToken cancellationToken)
        {
            var id = walletId.ToString();

            var existing = await _repository.GetAsync(id, cancellationToken)
                ?? throw new InvalidOperationException("Проебали");

            existing.Balance = newBalance;

            await _repository.IndexAsync(id, existing, cancellationToken);
        }
    }
}
