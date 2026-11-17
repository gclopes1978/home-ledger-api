using HomeLedger.Infrastructure.Data;
using HomeLedger.IntegrationTests.Stubs;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace HomeLedger.IntegrationTests;

public sealed class ApiFactory : WebApplicationFactory<Program>
{
    private readonly string _databaseName =
        $"HomeLedgerIntegrationTests_{Guid.NewGuid():N}";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        var password = Environment.GetEnvironmentVariable(
            "MSSQL_SA_PASSWORD");

        if (string.IsNullOrWhiteSpace(password))
        {
            throw new InvalidOperationException(
                "The MSSQL_SA_PASSWORD environment variable is not configured.");
        }

        var connectionStringBuilder = new SqlConnectionStringBuilder
        {
            DataSource = "127.0.0.1,1433",
            InitialCatalog = _databaseName,
            UserID = "sa",
            Password = password,
            Encrypt = true,
            TrustServerCertificate = true,
            MultipleActiveResultSets = false
        };

        builder.UseEnvironment("Testing");

        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<
                IDbContextOptionsConfiguration<HomeLedgerDbContext>>();

            services.RemoveAll<DbContextOptions<HomeLedgerDbContext>>();

            services.AddDbContext<HomeLedgerDbContext>(options =>
                options.UseSqlServer(
                    connectionStringBuilder.ConnectionString));
        });
    }

    public async Task SeedAsync(CancellationToken cancellationToken)
    {
        await using var scope = Services.CreateAsyncScope();

        var db = scope.ServiceProvider
            .GetRequiredService<HomeLedgerDbContext>();

        if (await db.Groupings.AnyAsync(cancellationToken))
        {
            return;
        }

        var grouping = GroupingStub.Create();
        var category = CategoryStub.Create(grouping);
        var source = SourceStub.Create();
        var paymentType = PaymentTypeStub.Create();

        db.Groupings.Add(grouping);
        db.Categories.Add(category);
        db.Sources.Add(source);
        db.PaymentTypes.Add(paymentType);

        db.Expenses.AddRange(
            ExpenseStub.Create(source, category, paymentType),
            ExpenseStub.Create(source, category, paymentType, new DateOnly(2025, 12, 31)),
            ExpenseStub.Create(source, category, paymentType, new DateOnly(2026, 1, 31)),
            ExpenseStub.Create(source, category, paymentType, new DateOnly(2026, 2, 1)));

        db.Incomes.AddRange(
            IncomeStub.Create(),
            IncomeStub.Create(new DateOnly(2025, 12, 31)),
            IncomeStub.Create(new DateOnly(2026, 1, 31)),
            IncomeStub.Create(new DateOnly(2026, 2, 1)));

        await db.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteDatabaseAsync()
    {
        var password = Environment.GetEnvironmentVariable(
            "MSSQL_SA_PASSWORD");

        if (string.IsNullOrWhiteSpace(password))
        {
            throw new InvalidOperationException(
                "The MSSQL_SA_PASSWORD environment variable is not configured.");
        }

        var connectionString = new SqlConnectionStringBuilder
        {
            DataSource = "127.0.0.1,1433",
            InitialCatalog = "master",
            UserID = "sa",
            Password = password,
            Encrypt = true,
            TrustServerCertificate = true
        };

        await using var connection =
            new SqlConnection(connectionString.ConnectionString);

        await connection.OpenAsync();

        await using var command = connection.CreateCommand();

        command.CommandText = $"""
        IF DB_ID(N'{_databaseName}') IS NOT NULL
        BEGIN
            ALTER DATABASE [{_databaseName}]
            SET SINGLE_USER WITH ROLLBACK IMMEDIATE;

            DROP DATABASE [{_databaseName}];
        END
        """;

        await command.ExecuteNonQueryAsync();
    }
}