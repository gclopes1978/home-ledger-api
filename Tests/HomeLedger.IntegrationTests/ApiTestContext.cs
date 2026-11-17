using HomeLedger.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace HomeLedger.IntegrationTests;

public sealed class ApiTestContext : IAsyncDisposable
{
    private readonly ApiFactory _factory;

    private ApiTestContext(ApiFactory factory, HttpClient client)
    {
        _factory = factory;
        Client = client;
    }

    public HttpClient Client { get; }

    public static async Task<ApiTestContext> CreateAsync(CancellationToken cancellationToken)
    {
        var factory = new ApiFactory();
        var client = factory.CreateClient();
        await factory.SeedAsync(cancellationToken);

        return new ApiTestContext(factory, client);
    }

    public async ValueTask DisposeAsync()
    {
        Client.Dispose();

        await _factory.DisposeAsync();

        await _factory.DeleteDatabaseAsync();
    }

    public async Task<int> CountCategoriesByNameAsync(string name,  CancellationToken cancellationToken)
    {
        await using var scope = _factory.Services.CreateAsyncScope();

        var db = scope.ServiceProvider
            .GetRequiredService<HomeLedgerDbContext>();

        return await db.Categories
            .CountAsync(category => category.Name == name, cancellationToken);
    }

    public async Task DeleteAllExpensesAsync(CancellationToken cancellationToken)
    {
        await using var scope = _factory.Services.CreateAsyncScope();

        var db = scope.ServiceProvider
            .GetRequiredService<HomeLedgerDbContext>();

        await db.Expenses.ExecuteDeleteAsync(cancellationToken);
    }

    public async Task DeleteAllCategoriesAsync(CancellationToken cancellationToken)
    {
        await using var scope = _factory.Services.CreateAsyncScope();

        var db = scope.ServiceProvider
            .GetRequiredService<HomeLedgerDbContext>();

        await db.Categories.ExecuteDeleteAsync(cancellationToken);
    }
}
