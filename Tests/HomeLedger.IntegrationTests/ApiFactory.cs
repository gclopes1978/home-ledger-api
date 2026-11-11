using HomeLedger.Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace HomeLedger.IntegrationTests;

public sealed class ApiFactory : WebApplicationFactory<Program>
{
    private readonly string _databaseName = $"HomeLedgerIntegrationTests-{Guid.NewGuid():N}";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<IDbContextOptionsConfiguration<HomeLedgerDbContext>>();
            services.RemoveAll<DbContextOptions<HomeLedgerDbContext>>();
            services.AddDbContext<HomeLedgerDbContext>(options => options
                .UseInMemoryDatabase(_databaseName)
                .ConfigureWarnings(warnings => warnings.Ignore(InMemoryEventId.TransactionIgnoredWarning)));
        });
    }

    public async Task SeedAsync(CancellationToken cancellationToken)
    {
        await using var scope = Services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<HomeLedgerDbContext>();

        if (await db.Groupings.AnyAsync(cancellationToken))
        {
            return;
        }

        db.Groupings.Add(GroupingStub.Create());
        db.Categories.Add(CategoryStub.Create());
        db.Locations.Add(LocationStub.Create());
        db.PaymentTypes.Add(PaymentTypeStub.Create());
        db.Expenses.AddRange(
            ExpenseStub.Create(),
            ExpenseStub.Create(2, new DateOnly(2025, 12, 31)),
            ExpenseStub.Create(3, new DateOnly(2026, 1, 31)),
            ExpenseStub.Create(4, new DateOnly(2026, 2, 1)));
        db.Incomes.AddRange(
            IncomeStub.Create(),
            IncomeStub.Create(2, new DateOnly(2025, 12, 31)),
            IncomeStub.Create(3, new DateOnly(2026, 1, 31)),
            IncomeStub.Create(4, new DateOnly(2026, 2, 1)));

        await db.SaveChangesAsync(cancellationToken);
    }
}
