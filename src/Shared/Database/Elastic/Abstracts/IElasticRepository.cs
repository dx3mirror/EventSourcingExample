using Elastic.Clients.Elasticsearch;

namespace Shared.Database.Elastic.Abstracts
{
    /// <summary>
    /// Репозиторий для работы с Elasticsearch по типу документа <typeparamref name="TDocument"/>.
    /// Предоставляет базовые операции индексирования, чтения, удаления, поиска и bulk-запросов.
    /// </summary>
    public interface IElasticRepository<TDocument> where TDocument : class
    {
        /// <summary>
        /// Индексирует (создает или обновляет) документ с указанным идентификатором.
        /// </summary>
        /// <param name="id">Строковый идентификатор документа в индексе.</param>
        /// <param name="document">Экземпляр документа для записи.</param>
        /// <param name="cancellationToken">Токен отмены операции.</param>
        Task IndexAsync(string id, TDocument document, CancellationToken cancellationToken);

        /// <summary>
        /// Возвращает документ по идентификатору или <c>null</c>, если документ не найден (HTTP 404).
        /// </summary>
        /// <param name="id">Строковый идентификатор документа в индексе.</param>
        /// <param name="cancellationToken">Токен отмены операции.</param>
        /// <returns>Документ <typeparamref name="TDocument"/> или <c>null</c>, если не найден.</returns>
        Task<TDocument?> GetAsync(string id, CancellationToken cancellationToken);

        /// <summary>
        /// Удаляет документ по идентификатору.
        /// Возвращает <c>false</c>, если документ не найден (HTTP 404).
        /// </summary>
        /// <param name="id">Строковый идентификатор документа в индексе.</param>
        /// <param name="cancellationToken">Токен отмены операции.</param>
        /// <returns><c>true</c>, если документ удален; <c>false</c>, если не найден.</returns>
        Task<bool> DeleteAsync(string id, CancellationToken cancellationToken);

        /// <summary>
        /// Выполняет поиск по индексу, используя делегат конфигурации запроса.
        /// </summary>
        /// <param name="configureRequest">Делегат, настраивающий параметры запроса поиска.</param>
        /// <param name="cancellationToken">Токен отмены операции.</param>
        /// <returns>Ответ Elasticsearch с результатами поиска.</returns>
        Task<SearchResponse<TDocument>> SearchAsync(
            Action<SearchRequestDescriptor<TDocument>> configureRequest,
            CancellationToken cancellationToken);

        /// <summary>
        /// Выполняет пакетный (bulk) запрос к индексу, используя делегат конфигурации.
        /// </summary>
        /// <param name="configureRequest">Делегат, настраивающий операции bulk-запроса.</param>
        /// <param name="cancellationToken">Токен отмены операции.</param>
        /// <returns>Ответ Elasticsearch по результатам bulk-запроса.</returns>
        Task<BulkResponse> BulkAsync(
            Action<BulkRequestDescriptor> configureRequest,
            CancellationToken cancellationToken);
    }
}
