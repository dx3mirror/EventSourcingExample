using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PaymentService.Hosts.Migrations;
using PaymentService.Infrastructures.Contexts;
using Shared.Database.Migrations;
using Shared.Database.Migrations.Abstracts;
using System.Reflection;

var app = Host.CreateDefaultBuilder(args)
.ConfigureServices((hostContext, services) =>
{
    var connectionString = hostContext.Configuration.GetConnectionString("PaymentDb");
    if (string.IsNullOrEmpty(connectionString))
    {
        connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING");
    }

    services.AddDbContext<PaymentDbContext>(dbContextBuilder =>
    dbContextBuilder.UseNpgsql(connectionString, b => b.MigrationsAssembly(Assembly.GetExecutingAssembly().FullName)));

    services.AddSingleton<IDatabaseMigrationService, DatabaseMigrationService>();

    services.AddSingleton<Startup>();
})
.Build();

var migration = app.Services.GetService<Startup>()
?? throw new InvalidCastException("Не удалось получить экземпляр Startup");

using var cancellationTokenSource = new CancellationTokenSource();

await migration.StartMigrationsAsync(cancellationTokenSource.Token);