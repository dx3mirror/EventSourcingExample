using Elastic.Clients.Elasticsearch;
using Elastic.Transport.Products.Elasticsearch;
using Microsoft.Extensions.Logging;
using Shared.Database.Elastic.Abstracts;

namespace Shared.Database.Elastic
{
    public sealed class ElasticRepository<TDocument> : IElasticRepository<TDocument>
        where TDocument : class
    {
        private readonly ElasticsearchClient _client;
        private readonly IIndexNameResolver<TDocument> _indexNameResolver;
        private readonly ILogger<ElasticRepository<TDocument>>? _logger;

        public ElasticRepository(
            ElasticsearchClient client,
            IIndexNameResolver<TDocument> indexNameResolver,
            ILogger<ElasticRepository<TDocument>>? logger = null)
        {
            ArgumentNullException.ThrowIfNull(client);
            ArgumentNullException.ThrowIfNull(indexNameResolver);

            _client = client;
            _indexNameResolver = indexNameResolver;
            _logger = logger;
        }

        /// <summary>Indexes or creates a document with the specified id.</summary>
        public async Task IndexAsync(string id, TDocument document, CancellationToken cancellationToken)
        {
            ArgumentException.ThrowIfNullOrEmpty(id);
            ArgumentNullException.ThrowIfNull(document);

            var response = await _client.IndexAsync(
                document,
                d => d.Index(GetIndexName()).Id(id),
                cancellationToken
            );

            EnsureSuccess(response, "INDEX");
        }

        /// <summary>Gets a document by id or returns <c>null</c> if not found (404).</summary>
        public async Task<TDocument?> GetAsync(string id, CancellationToken cancellationToken)
        {
            ArgumentException.ThrowIfNullOrEmpty(id);

            var response = await _client.GetAsync<TDocument>(
                id,
                d => d.Index(GetIndexName()),
                cancellationToken
            );

            if (response.ApiCallDetails?.HttpStatusCode == 404)
            {
                return null;
            }

            if (response.IsSuccess())
            {
                return response.Found ? response.Source : null;
            }

            ThrowElasticsearchOperationException(response, "GET");
            return null;
        }

        /// <summary>Deletes a document by id. Returns <c>false</c> for 404.</summary>
        public async Task<bool> DeleteAsync(string id, CancellationToken cancellationToken)
        {
            ArgumentException.ThrowIfNullOrEmpty(id);

            var response = await _client.DeleteAsync<TDocument>(
                id,
                d => d.Index(GetIndexName()),
                cancellationToken
            );

            if (response.ApiCallDetails?.HttpStatusCode == 404)
            {
                return false;
            }

            if (response.IsSuccess())
            {
                return response.Result == Result.Deleted;
            }

            ThrowElasticsearchOperationException(response, "DELETE");
            return false;
        }

        /// <summary>Executes a search against the resolved index.</summary>
        public async Task<SearchResponse<TDocument>> SearchAsync(
            Action<SearchRequestDescriptor<TDocument>> configure,
            CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(configure);

            var response = await _client.SearchAsync<TDocument>(
                s =>
                {
                    s.Indices(GetIndexName());
                    configure(s);
                },
                cancellationToken
            );

            EnsureSuccess(response, "SEARCH");
            return response;
        }

        /// <summary>Executes a bulk request against the resolved index.</summary>
        public async Task<BulkResponse> BulkAsync(
            Action<BulkRequestDescriptor> configure,
            CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(configure);

            var response = await _client.BulkAsync(
                b =>
                {
                    b.Index(GetIndexName());
                    configure(b);
                },
                cancellationToken
            );

            EnsureSuccess(response, "BULK");
            return response;
        }

        private string GetIndexName()
        {
            return _indexNameResolver.Resolve();
        }

        private void EnsureSuccess(ElasticsearchResponse response, string operation)
        {
            if (response.IsSuccess())
            {
                return;
            }

            _logger?.LogError(
                "Elasticsearch {Operation} failed. StatusCode: {StatusCode}. Error: {Error}",
                operation,
                response.ApiCallDetails?.HttpStatusCode,
                response.ElasticsearchServerError?.Error?.Reason ?? response.ApiCallDetails?.ToString() ?? "unknown"
            );

            ThrowElasticsearchOperationException(response, operation);
        }

        private static void ThrowElasticsearchOperationException(ElasticsearchResponse response, string operation)
        {
            var statusCode = response.ApiCallDetails?.HttpStatusCode;
            var reason =
                response.ElasticsearchServerError?.Error?.Reason
                ?? response.ApiCallDetails?.ToString()
                ?? "unknown";

            throw new InvalidOperationException($"Elasticsearch {operation} failed (status {statusCode}): {reason}");
        }
    }
}
