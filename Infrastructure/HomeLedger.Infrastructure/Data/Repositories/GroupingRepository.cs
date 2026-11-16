using AutoMapper;
using HomeLedger.Domain.BusinessModels;
using HomeLedger.Domain.Interfaces.Repositories;
using HomeLedger.Infrastructure.Data.Interfaces.Groupings;
using HomeLedger.Infrastructure.Entities;

namespace HomeLedger.Infrastructure.Data.Repositories;

public sealed class GroupingRepository(
    HomeLedgerDbContext db,
    IGroupingData data,
    IMapper mapper) : IGroupingRepository
{
    public async Task<IReadOnlyList<Grouping>> GetAllAsync(CancellationToken ct)
    {
        var entities = await data.GetAllAsync(ct);
        return mapper.Map<IReadOnlyList<Grouping>>(entities);
    }

    public async Task<Grouping?> GetAsync(int id, CancellationToken ct)
    {
        var entity = await data.GetAsync(id, ct);
        return entity is null ? null : mapper.Map<Grouping>(entity);
    }

    public async Task<Grouping> InsertAsync(Grouping model, CancellationToken ct)
    {
        await using var transaction = await db.Database.BeginTransactionAsync(ct);
        try
        {
            var entity = mapper.Map<GroupingEntity>(model);
            var insertedEntity = await data.InsertAsync(entity, ct);
            await transaction.CommitAsync(ct);
            return mapper.Map<Grouping>(insertedEntity);
        }
        catch
        {
            await transaction.RollbackAsync(ct);
            throw;
        }
    }

    public async Task<bool> UpdateAsync(int id, Grouping model, CancellationToken ct)
    {
        await using var transaction = await db.Database.BeginTransactionAsync(ct);
        try
        {
            model.Id = id;
            var entity = mapper.Map<GroupingEntity>(model);
            var updated = await data.UpdateAsync(id, entity, ct);
            if (!updated)
            {
                await transaction.RollbackAsync(ct);
                return false;
            }

            await transaction.CommitAsync(ct);
            return true;
        }
        catch
        {
            await transaction.RollbackAsync(ct);
            throw;
        }
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct)
    {
        await using var transaction = await db.Database.BeginTransactionAsync(ct);
        try
        {
            var deleted = await data.DeleteAsync(id, ct);
            if (!deleted)
            {
                await transaction.RollbackAsync(ct);
                return false;
            }

            await transaction.CommitAsync(ct);
            return true;
        }
        catch
        {
            await transaction.RollbackAsync(ct);
            throw;
        }
    }
}
