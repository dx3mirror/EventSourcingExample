using PaymentService.Infrastructures.Contexts.StoreEntity;

namespace PaymentService.Persistence.Providers.Abstracts
{
    public interface IEventStoreProvider
    {
        /// <summary>
        /// Прочитать события стрима (агрегата) строго по возрастанию версии.
        /// </summary>
        Task<IReadOnlyList<PaymentStore>> ReadStreamAsync(
            Guid aggregateId,
            CancellationToken cancellationToken,
            int fromExclusiveVersion = -1);

        /// <summary>
        /// Добавить пачку событий в стрим с оптимистичной блокировкой (expectedVersion).
        /// Вернёт вставленные строки (с присвоенными Version/EventId/CreatedUtc).
        /// </summary>
        Task<IReadOnlyList<PaymentStore>> AppendAsync(
            Guid aggregateId,
            int expectedVersion,
            IEnumerable<(string Type, string PayloadJson, string? MetadataJson)> events,
            CancellationToken cancellationToken);
    }
}
