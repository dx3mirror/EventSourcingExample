using PaymentService.Handlers.Commands.WalletDeposit;
using Wolverine;
using PaymentService.Providers;
using PaymentService.Persistence.Converters.Abstracts;
using PaymentService.Persistence.Converters;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddWolverine(opts =>
{
    opts.Discovery.IncludeAssembly(typeof(WalletDepositCommandHandler).Assembly);
});
builder.Services.AddPostgreDb();
builder.Services.AddSingleton<IEventSerializer, JsonEventSerializer>();
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
