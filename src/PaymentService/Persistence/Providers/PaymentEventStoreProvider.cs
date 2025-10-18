using Microsoft.EntityFrameworkCore;
using PaymentService.Infrastructures.Contexts.StoreEntity;
using PaymentService.Persistence.Providers.Abstracts;
using Shared.Database.Postgres.Repositories;

namespace PaymentService.Persistence.Providers
{
    public class PaymentEventStoreProvider(IRepository<PaymentStore> repository) : IEventStoreProvider
    {
        private readonly IRepository<PaymentStore> _repository = repository;

        public async Task<IReadOnlyList<PaymentStore>> ReadStreamAsync(
            Guid aggregateId, CancellationToken cancellationToken, int fromExclusiveVersion = -1)
        {
            return await _repository
                .Where(x => x.AggregateId == aggregateId && x.Version > fromExclusiveVersion)
                .AsNoTracking()
                .OrderBy(x => x.Version)
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<PaymentStore>> AppendAsync(
            Guid aggregateId,
            int expectedVersion,
            IEnumerable<(string Type, string PayloadJson, string? MetadataJson)> events,
            CancellationToken cancellationToken)
        {
            // 1) Проверка оптимистичной блокировки
            var current = await _repository
                .Where(x => x.AggregateId == aggregateId)
                .MaxAsync(x => (int?)x.Version, cancellationToken) ?? -1;

            if (current != expectedVersion)
            {
                throw new InvalidOperationException($"WrongExpectedVersion: expected {expectedVersion}, actual {current}");
            }

            // 2) Подготовка вставок
            var next = current;
            var now = DateTime.UtcNow;
            var batch = new List<PaymentStore>();

            foreach (var (type, payload, meta) in events)
            {
                next += 1;
                batch.Add(new PaymentStore
                {
                    AggregateId = aggregateId,
                    Version = next,
                    Type = type,
                    Payload = payload,
                    Metadata = meta ?? "{}",
                    CreatedAt = now,
                    EventId = Guid.NewGuid()
                });
            }

            if (batch.Count == 0) return [];

            await _repository.AddRangeAsync([..batch], cancellationToken);

            return batch;
        }
    }
}
