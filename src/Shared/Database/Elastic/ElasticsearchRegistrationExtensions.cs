using System;
using System.Linq;
using Elastic.Clients.Elasticsearch;
using Elastic.Transport;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Shared.Database.Elastic.Abstracts;
using Shared.Database.Elastic.Options;

namespace Shared.Database.Elastic
{
    public static class ElasticsearchRegistrationExtensions
    {
        public static IServiceCollection AddElasticsearch(
            this IServiceCollection services,
            string uri = "http://localhost:9200",
            bool enableDebug = true,
            string indexPrefix = "dev-")
        {
            // 1) Регистрируем ElasticOptions, т.к. его требует DefaultIndexNameResolver<T>
            services.AddSingleton(new ElasticOptions
            {
                NodeUris = uri,
                EnableDebugLogging = enableDebug,
                IndexPrefix = indexPrefix,
                RequestTimeoutSeconds = 30
            });

            // 2) Клиент
            services.AddSingleton(sp =>
            {
                var logger = sp.GetRequiredService<ILoggerFactory>().CreateLogger("ElasticHTTP");

                var settings = new ElasticsearchClientSettings(new Uri(uri))
                    .RequestTimeout(TimeSpan.FromSeconds(30));

                if (enableDebug)
                {
                    settings = settings.EnableDebugMode(details =>
                    {
                        if (details.RequestBodyInBytes is not null)
                            logger.LogInformation("ES Request: {Method} {Uri}\n{Body}",
                                details.HttpMethod, details.Uri, System.Text.Encoding.UTF8.GetString(details.RequestBodyInBytes));
                        if (details.ResponseBodyInBytes is not null)
                            logger.LogInformation("ES Response: {Code}\n{Body}",
                                details.HttpStatusCode, System.Text.Encoding.UTF8.GetString(details.ResponseBodyInBytes));
                    }).PrettyJson();
                }

                return new ElasticsearchClient(settings);
            });

            // 3) Остальные сервисы
            services.AddSingleton(typeof(IIndexNameResolver<>), typeof(DefaultIndexNameResolver<>));
            services.AddScoped(typeof(IElasticRepository<>), typeof(ElasticRepository<>));

            return services;
        }
    }
}
