using HomeLedger.Infrastructure.Data.Factories;
using HomeLedger.Infrastructure.Data.Interfaces.Idempotency;
using HomeLedger.Infrastructure.Entities;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace HomeLedger.Infrastructure.Data.Implementations;

public sealed class IdempotencyRecordData(HomeLedgerDbContext db)
    : DataFactory<IdempotencyRecordEntity>(db), IIdempotencyData
{
    public async Task<bool> TryInsertAsync(
        IdempotencyRecordEntity entity,
        CancellationToken ct)
    {
        if (await GetByKeyAsync(entity.IdempotencyKey, ct) is not null)
        {
            return false;
        }

        Entities.Add(entity);

        try
        {
            await DbContext.SaveChangesAsync(ct);
            return true;
        }
        catch (DbUpdateException exception)
        {
            DbContext.Entry(entity).State = EntityState.Detached;

            if (!IsUniqueConstraintViolation(exception))
            {
                throw;
            }

            var existing = await GetByKeyAsync(entity.IdempotencyKey, ct);

            if (existing is null)
            {
                throw;
            }

            return false;
        }
    }

    public async Task<IdempotencyRecordEntity?> GetByKeyAsync(
        string idempotencyKey,
        CancellationToken ct)
    {
        return await Entities
            .AsNoTracking()
            .SingleOrDefaultAsync(
                record => record.IdempotencyKey == idempotencyKey,
                ct);
    }

    public async Task CompleteAsync(
        string idempotencyKey,
        int responseStatusCode,
        string? responseContentType,
        string? responseSource,
        string responseBody,
        CancellationToken ct)
    {
        var record = await Entities
            .SingleOrDefaultAsync(
                entry => entry.IdempotencyKey == idempotencyKey,
                ct)
            ?? throw new InvalidOperationException(
                $"The idempotency record '{idempotencyKey}' was not found.");

        record.ResponseStatusCode = responseStatusCode;
        record.ResponseContentType = responseContentType;
        record.ResponseLocation = responseSource;
        record.ResponseBody = responseBody;

        await DbContext.SaveChangesAsync(ct);
    }

    public async Task DeleteByKeyAsync(
        string idempotencyKey,
        CancellationToken ct)
    {
        var record = await Entities
            .SingleOrDefaultAsync(
                entry => entry.IdempotencyKey == idempotencyKey,
                ct);

        if (record is null)
        {
            return;
        }

        Entities.Remove(record);
        await DbContext.SaveChangesAsync(ct);
    }

    private static bool IsUniqueConstraintViolation(
        DbUpdateException exception)
    {
        return exception.GetBaseException() is SqlException sqlException
            && (sqlException.Number == 2601
                || sqlException.Number == 2627);
    }
}