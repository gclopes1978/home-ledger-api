using HomeLedger.Infrastructure.Entities;

namespace HomeLedger.Infrastructure.Data.Interfaces.Idempotency;

public interface IIdempotencyData : IData<IdempotencyRecordEntity>
{
    Task<bool> TryInsertAsync(IdempotencyRecordEntity entity, CancellationToken ct);

    Task<IdempotencyRecordEntity?> GetByKeyAsync(string idempotencyKey, CancellationToken ct);

    Task CompleteAsync(
        string idempotencyKey,
        int responseStatusCode,
        string? responseContentType,
        string? responseSource,
        string responseBody,
        CancellationToken ct);

    Task DeleteByKeyAsync(string idempotencyKey, CancellationToken ct);
}