using Shared.Database.Elastic.Abstracts;
using Shared.Database.Elastic.Options;

namespace Shared.Database.Elastic
{
    /// <summary>Стратегия по умолчанию.</summary>
    public sealed class DefaultIndexNameResolver<TDocument> : IIndexNameResolver<TDocument>
    {
        private readonly ElasticOptions _options;
        public DefaultIndexNameResolver(ElasticOptions options) => _options = options;

        public string Resolve()
        {
            var type = typeof(TDocument).Name;
            var kebab = ToKebab(type);
            return string.IsNullOrWhiteSpace(_options.IndexPrefix)
                ? kebab
                : $"{_options.IndexPrefix}{kebab}";
        }

        private static string ToKebab(string s)
        {
            System.Text.StringBuilder sb = new();
            for (var i = 0; i < s.Length; i++)
            {
                var c = s[i];
                if (char.IsUpper(c) && i > 0) sb.Append('-');
                sb.Append(char.ToLowerInvariant(c));
            }
            return sb.ToString();
        }
    }
}
