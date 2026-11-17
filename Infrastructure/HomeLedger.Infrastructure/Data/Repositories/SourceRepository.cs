using AutoMapper;
using HomeLedger.Domain.BusinessModels;
using HomeLedger.Domain.Interfaces.Repositories;
using HomeLedger.Infrastructure.Data.Interfaces.Sources;
using HomeLedger.Infrastructure.Data.Transactions;
using HomeLedger.Infrastructure.Entities;

namespace HomeLedger.Infrastructure.Data.Repositories;

public sealed class SourceRepository(
    HomeLedgerDbContext db,
    ISourceData data,
    IMapper mapper) : ISourceRepository
{
    public async Task<IReadOnlyList<Source>> GetAllAsync(CancellationToken ct)
    {
        var entities = await data.GetAllAsync(ct);
        return mapper.Map<IReadOnlyList<Source>>(entities);
    }

    public async Task<Source?> GetAsync(int id, CancellationToken ct)
    {
        var entity = await data.GetAsync(id, ct);
        return entity is null ? null : mapper.Map<Source>(entity);
    }

    public async Task<Source> InsertAsync(Source model, CancellationToken ct)
    {
        await using var transaction =
            await RepositoryTransaction.BeginAsync(db, ct);

        try
        {
            var entity = mapper.Map<SourceEntity>(model);
            var insertedEntity = await data.InsertAsync(entity, ct);

            await transaction.CommitAsync(ct);

            return mapper.Map<Source>(insertedEntity);
        }
        catch
        {
            await transaction.RollbackAsync(CancellationToken.None);
            throw;
        }
    }

    public async Task<bool> UpdateAsync(
        int id,
        Source model,
        CancellationToken ct)
    {
        await using var transaction =
            await RepositoryTransaction.BeginAsync(db, ct);

        try
        {
            model.Id = id;

            var entity = mapper.Map<SourceEntity>(model);
            var updated = await data.UpdateAsync(id, entity, ct);

            if (!updated)
            {
                await transaction.RollbackAsync(CancellationToken.None);
                return false;
            }

            await transaction.CommitAsync(ct);
            return true;
        }
        catch
        {
            await transaction.RollbackAsync(CancellationToken.None);
            throw;
        }
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct)
    {
        await using var transaction =
            await RepositoryTransaction.BeginAsync(db, ct);

        try
        {
            var deleted = await data.DeleteAsync(id, ct);

            if (!deleted)
            {
                await transaction.RollbackAsync(CancellationToken.None);
                return false;
            }

            await transaction.CommitAsync(ct);
            return true;
        }
        catch
        {
            await transaction.RollbackAsync(CancellationToken.None);
            throw;
        }
    }
}