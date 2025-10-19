namespace Shared.Database.Elastic.Abstracts
{
    public interface IIndexNameResolver<TDocument>
    {
        /// <summary>Имя индекса для типа документа.</summary>
        string Resolve();
    }
}
