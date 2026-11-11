using HomeLedger.Application.Extensions;
using HomeLedger.Api.Mapping;
using HomeLedger.Infrastructure.Data;
using HomeLedger.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddAutoMapper(_ => { }, typeof(ApiMappingProfile));
builder.Services.AddHomeLedgerApplication();
builder.Services.AddHomeLedgerInfrastructure(builder.Configuration);

var app = builder.Build();
await using (var scope = app.Services.CreateAsyncScope())
{
    var db = scope.ServiceProvider.GetRequiredService<HomeLedgerDbContext>();
    if (db.Database.IsRelational())
    {
        await db.Database.MigrateAsync();
    }
    else
    {
        await db.Database.EnsureCreatedAsync();
    }
}
if (app.Environment.IsDevelopment()) app.MapOpenApi();
app.MapControllers();
await app.RunAsync();
