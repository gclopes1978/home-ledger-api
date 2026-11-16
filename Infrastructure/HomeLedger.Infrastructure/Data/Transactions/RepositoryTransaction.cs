using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace HomeLedger.Infrastructure.Data.Transactions;

public sealed class RepositoryTransaction : IAsyncDisposable
{
    private readonly IDbContextTransaction? transaction;

    private RepositoryTransaction(IDbContextTransaction? transaction)
    {
        this.transaction = transaction;
    }

    public static async Task<RepositoryTransaction> BeginAsync(
        DbContext dbContext,
        CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(dbContext);

        if (dbContext.Database.CurrentTransaction is not null)
        {
            return new RepositoryTransaction(null);
        }

        var transaction = await dbContext.Database.BeginTransactionAsync(ct);

        return new RepositoryTransaction(transaction);
    }

    public Task CommitAsync(CancellationToken ct = default)
    {
        return transaction?.CommitAsync(ct) ?? Task.CompletedTask;
    }

    public Task RollbackAsync(CancellationToken ct = default)
    {
        return transaction?.RollbackAsync(ct) ?? Task.CompletedTask;
    }

    public ValueTask DisposeAsync()
    {
        return transaction?.DisposeAsync() ?? ValueTask.CompletedTask;
    }
}