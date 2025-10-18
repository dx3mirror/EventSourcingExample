using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure;
using Shared.Database.Postgres.Configurations;
using Shared.Database.Postgres.Repositories;
using Shared.Database.Postgres.Session;

namespace Kortros.Common.Infrastructures.DataAccess.Extensions
{
    /// <summary>
    /// Регистрация доступа к базе данных.
    /// </summary>
    public static class DataAccessRegistrar
    {
        /// <summary>
        /// Регистрирует классы для доступа к базе данных.
        /// </summary>
        /// <typeparam name="TDbContext">Контекст базы данных.</typeparam>
        /// <typeparam name="TDbContextConfigurator">Конфигуратор базы данных.</typeparam>
        /// <param name="services">Коллекция сервисов.</param>
        /// <returns>Коллекция сервисов.</returns>
        public static IServiceCollection AddDataAccess<TDbContext, TDbContextConfigurator>(
            this IServiceCollection services)
            where TDbContext : DbContext
            where TDbContextConfigurator : class, IDbContextOptionsConfigurator<TDbContext>
        {
            services.AddDataAccess<TDbContext, TDbContextConfigurator>(_ => {});
            
            return services;
        }
        
        /// <summary>
        /// Регистрирует классы для доступа к базе данных.
        /// </summary>
        /// <typeparam name="TDbContext">Контекст базы данных.</typeparam>
        /// <typeparam name="TDbContextConfigurator">Конфигуратор базы данных.</typeparam>
        /// <param name="services">Коллекция сервисов.</param>
        /// <param name="npgsqlOptionsAction">Дополнительная настройки postgres</param>
        /// <returns>Коллекция сервисов.</returns>
        public static IServiceCollection AddDataAccess<TDbContext, TDbContextConfigurator>(
            this IServiceCollection services,
            Action<NpgsqlDbContextOptionsBuilder> npgsqlOptionsAction)
            where TDbContext : DbContext
            where TDbContextConfigurator : class, IDbContextOptionsConfigurator<TDbContext>
        {
            services.AddEntityFrameworkNpgsql()
               .AddDbContextPool<TDbContext>((a, b) => Configure<TDbContext>(a, b, npgsqlOptionsAction));

            services
                .AddSingleton<IDbContextOptionsConfigurator<TDbContext>, TDbContextConfigurator>()
                .AddScoped<DbContext>(sp => sp.GetRequiredService<TDbContext>())
                .AddScoped<IDataSessionFactory, EntityFrameworkDataSessionFactory>()
                .AddScoped(typeof(IRepository<>), typeof(EntityFrameworkRepository<>));

            return services;
        }

        internal static void Configure<TDbContext>(
            IServiceProvider sp,
            DbContextOptionsBuilder dbOptions,
            Action<NpgsqlDbContextOptionsBuilder> npgsqlOptionsAction) where TDbContext : DbContext
        {
            var configurator = sp.GetRequiredService<IDbContextOptionsConfigurator<TDbContext>>();
            configurator.Configure((DbContextOptionsBuilder<TDbContext>)dbOptions, npgsqlOptionsAction);
        }
    }
}
