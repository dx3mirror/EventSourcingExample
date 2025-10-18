namespace Shared.Database.Postgres.Session
{
    /// <summary>
    /// Интерфейс фабрики для создания <see cref="IDataSession"/>.
    /// </summary>
    public interface IDataSessionFactory
    {
        /// <summary>
        /// Создает новый экземпляр <see cref="IDataSession"/>.
        /// </summary>
        /// <returns>Сессия для доступа к данным.</returns>
        IDataSession Create();
    }
}
