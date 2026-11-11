using AutoMapper;
using HomeLedger.Domain.BusinessModels;
using HomeLedger.Domain.Interfaces.Repositories;
using HomeLedger.Infrastructure.Data.Interfaces.Locations;
using HomeLedger.Infrastructure.Entities;

namespace HomeLedger.Infrastructure.Data.Repositories;

public sealed class LocationRepository(
    HomeLedgerDbContext db,
    ILocationData data,
    IMapper mapper) : ILocationRepository
{
    public async Task<IReadOnlyList<Location>> GetAllAsync(CancellationToken ct)
    {
        var entities = await data.GetAllAsync(ct);
        return mapper.Map<IReadOnlyList<Location>>(entities);
    }

    public async Task<Location?> GetAsync(int id, CancellationToken ct)
    {
        var entity = await data.GetAsync(id, ct);
        return entity is null ? null : mapper.Map<Location>(entity);
    }

    public async Task<Location> InsertAsync(Location model, CancellationToken ct)
    {
        await using var transaction = await db.Database.BeginTransactionAsync(ct);
        try
        {
            var entity = mapper.Map<LocationEntity>(model);
            var insertedEntity = await data.InsertAsync(entity, ct);
            await transaction.CommitAsync(ct);
            return mapper.Map<Location>(insertedEntity);
        }
        catch
        {
            await transaction.RollbackAsync(ct);
            throw;
        }
    }

    public async Task<bool> UpdateAsync(int id, Location model, CancellationToken ct)
    {
        await using var transaction = await db.Database.BeginTransactionAsync(ct);
        try
        {
            model.Id = id;
            var entity = mapper.Map<LocationEntity>(model);
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
