using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Shared.Database.Postgres.Repositories
{
    /// <inheritdoc/>
    public class EntityFrameworkRepository<TEntity> : IRepository<TEntity>
        where TEntity : class
    {
        /// <summary>
        /// Контекст базы данных.
        /// </summary>
        protected DbContext DbContext { get; }

        /// <summary>
        /// Хранилище сущностей <typeparamref name="TEntity"/>.
        /// </summary>
        protected DbSet<TEntity> DbSet { get; }

        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="context">Контекст базы данных.</param>
        public EntityFrameworkRepository(DbContext context)
        {
            DbContext = context;
            DbSet = DbContext.Set<TEntity>();
        }

        /// <inheritdoc/>
        public IQueryable<TEntity> AsQueryable()
        {
            return DbSet.AsQueryable<TEntity>();
        }

        /// <inheritdoc/>
        public async Task AddAsync(TEntity entity, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(entity);

            await DbContext.AddAsync(entity, cancellationToken);

            await SaveChangesAsync(cancellationToken);
        }

        /// <inheritdoc/>
        public async Task AddRangeAsync(TEntity[] entities, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(entities);

            await DbContext.AddRangeAsync(entities, cancellationToken);

            await SaveChangesAsync(cancellationToken);
        }

        /// <inheritdoc />
        public IQueryable<TEntity> Where(Expression<Func<TEntity, bool>> expression)
        {
            ArgumentNullException.ThrowIfNull(expression);
            return DbSet.Where(expression);
        }

        /// <inheritdoc />
        public Task UpdateAsync(TEntity entity, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(entity);

            var state = DbContext.Entry(entity).State;
            if (state == EntityState.Detached)
            {
                DbContext.Attach(entity);
            }

            DbContext.Update(entity);

            return SaveChangesAsync(cancellationToken);
        }

        /// <inheritdoc />
        public async Task DeleteAsync(TEntity[] entities, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(entities);

            foreach (var entity in entities)
            {
                DbContext.Remove(entity);
            }

            await SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// Находит сущность по идентификатору.
        /// </summary>
        /// <typeparam name="TKey">Тип идентификатора сущности.</typeparam>
        /// <param name="id">Идентификатор сущности.</param>
        /// <param name="cancellationToken">Токен отмены для асинхронных операций.</param>
        /// <returns>Сущность <typeparamref name="TEntity"/> или null, если не найдена.</returns>
        public async Task<TEntity?> FindAsync<TKey>(TKey id, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(id);
            return await DbSet.FindAsync(id, cancellationToken);
        }

        /// <inheritdoc />
        public Task ExecuteRawSqlAsync(string sql, IReadOnlyCollection<object> parameters, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(sql);

            DbContext.Database.SqlQueryRaw<TEntity>(sql, parameters);

            return SaveChangesAsync(cancellationToken);
        }

        private Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            return DbContext.SaveChangesAsync(cancellationToken);
        }

        private static IOrderedQueryable<T> OrderByColumnUsing<T>(IQueryable<T> source, string columnPath, string method)
        {
            var parameter = Expression.Parameter(typeof(T), "item");
            var member = columnPath.Split('.')
                .Aggregate((Expression)parameter, Expression.PropertyOrField);
            var keySelector = Expression.Lambda(member, parameter);
            var methodCall = Expression.Call(typeof(Queryable), method, [parameter.Type, member.Type],
                source.Expression, Expression.Quote(keySelector));

            return (IOrderedQueryable<T>)source.Provider.CreateQuery(methodCall);
        }
    }
}
