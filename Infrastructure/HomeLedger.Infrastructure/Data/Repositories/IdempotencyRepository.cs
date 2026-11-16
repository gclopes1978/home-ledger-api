using AutoMapper;
using HomeLedger.Domain.BusinessModels;
using HomeLedger.Domain.Interfaces.Repositories;
using HomeLedger.Infrastructure.Data.Interfaces.Idempotency;
using HomeLedger.Infrastructure.Entities;

namespace HomeLedger.Infrastructure.Data.Repositories;

public sealed class IdempotencyRepository(
    IIdempotencyData data,
    IMapper mapper) : IIdempotencyRepository
{
    public async Task<bool> TryBeginAsync(IdempotencyRecord record, CancellationToken ct)
    {
        var entity = mapper.Map<IdempotencyRecordEntity>(record);
        return await data.TryInsertAsync(entity, ct);
    }

    public async Task<IdempotencyRecord?> GetByKeyAsync(
        string idempotencyKey,
        CancellationToken ct)
    {
        var entity = await data.GetByKeyAsync(idempotencyKey, ct);
        return entity is null ? null : mapper.Map<IdempotencyRecord>(entity);
    }

    public Task CompleteAsync(
        string idempotencyKey,
        int responseStatusCode,
        string? responseContentType,
        string? responseSource,
        string responseBody,
        CancellationToken ct)
    {
        return data.CompleteAsync(
            idempotencyKey,
            responseStatusCode,
            responseContentType,
            responseSource,
            responseBody,
            ct);
    }

    public Task DeleteAsync(string idempotencyKey, CancellationToken ct)
    {
        return data.DeleteByKeyAsync(idempotencyKey, ct);
    }
}