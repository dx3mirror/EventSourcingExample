namespace Shared.Database.Elastic.Options
{
    /// <summary>Опции подключения к Elasticsearch (общие, без привязки к конкретным индексам).</summary>
    public sealed class ElasticOptions
    {
        /// <summary>Elastic Cloud: Cloud ID (приоритетнее NodeUris).</summary>
        public string? CloudId { get; set; }

        /// <summary>Elastic Cloud/API: ApiKey (формат "id:api_key" или уже Base64).</summary>
        public string? ApiKey { get; set; }

        /// <summary>Самостоятельный кластер: список нод через запятую, например "http://localhost:9200,https://node2:9200".</summary>
        public string? NodeUris { get; set; }

        /// <summary>Базовая авторизация (если не используешь ApiKey).</summary>
        public string? Username { get; set; }

        public string? Password { get; set; }

        /// <summary>Отпечаток сертификата для pinning (Elastic Cloud выдаёт в админке).</summary>
        public string? CertificateFingerprint { get; set; }

        /// <summary>Префикс индексов по умолчанию, например "app-" → "app-users".</summary>
        public string? IndexPrefix { get; set; } = null;

        /// <summary>Таймаут HTTP-запросов.</summary>
        public int RequestTimeoutSeconds { get; set; } = 30;

        /// <summary>Включить подробные логи запросов/ответов (осторожно в проде).</summary>
        public bool EnableDebugLogging { get; set; } = false;
    }
}
