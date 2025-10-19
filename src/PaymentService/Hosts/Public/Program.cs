using PaymentService.Handlers.Commands.WalletDeposit;
using Wolverine;
using PaymentService.Providers;
using PaymentService.Persistence.Converters.Abstracts;
using PaymentService.Persistence.Converters;
using PaymentService.Persistence.Providers.Abstracts;
using PaymentService.Persistence.Providers;
using Shared.Database.Elastic;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddWolverine(opts =>
{
    opts.Discovery.IncludeAssembly(typeof(WalletDepositCommandHandler).Assembly);
});

builder.Services.AddPostgreDb();
builder.Services.AddMassTransitProduceProviders(builder.Configuration);
builder.Services.AddScoped<IEventSerializer, JsonEventSerializer>();
builder.Services.AddScoped<IEventStoreProvider, PaymentEventStoreProvider>();
builder.Services.AddElasticsearch("http://localhost:9200", enableDebug: true);
builder.Services.AddScoped<IWalletElasticProvider, WalletElasticProvider>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
await app.RunAsync();
