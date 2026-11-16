using HomeLedger.Domain.BusinessModels;
using HomeLedger.Domain.Interfaces.Application;
using HomeLedger.Domain.Interfaces.Repositories;

namespace HomeLedger.Application.UseCases;

public sealed class IdempotencyUseCase(IIdempotencyRepository repository)
    : IIdempotencyUseCase
{
    public Task<bool> TryBeginAsync(IdempotencyRecord record, CancellationToken ct)
    {
        return repository.TryBeginAsync(record, ct);
    }

    public Task<IdempotencyRecord?> GetByKeyAsync(string idempotencyKey, CancellationToken ct)
    {
        return repository.GetByKeyAsync(idempotencyKey, ct);
    }

    public Task CompleteAsync(
        string idempotencyKey,
        int responseStatusCode,
        string? responseContentType,
        string? responseSource,
        string responseBody,
        CancellationToken ct)
    {
        return repository.CompleteAsync(
            idempotencyKey,
            responseStatusCode,
            responseContentType,
            responseSource,
            responseBody,
            ct);
    }

    public Task DeleteAsync(string idempotencyKey, CancellationToken ct)
    {
        return repository.DeleteAsync(idempotencyKey, ct);
    }
}