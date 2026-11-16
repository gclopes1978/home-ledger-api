using HomeLedger.Domain.BusinessModels;

namespace HomeLedger.Domain.Interfaces.Repositories;

public interface IIdempotencyRepository
{
    Task<bool> TryBeginAsync(IdempotencyRecord record, CancellationToken ct);

    Task<IdempotencyRecord?> GetByKeyAsync(string idempotencyKey, CancellationToken ct);

    Task CompleteAsync(
        string idempotencyKey,
        int responseStatusCode,
        string? responseContentType,
        string? responseSource,
        string responseBody,
        CancellationToken ct);

    Task DeleteAsync(string idempotencyKey, CancellationToken ct);
}