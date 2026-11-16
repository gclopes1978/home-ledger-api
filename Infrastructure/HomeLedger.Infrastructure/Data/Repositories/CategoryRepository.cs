using AutoMapper;
using HomeLedger.Domain.BusinessModels;
using HomeLedger.Domain.Interfaces.Repositories;
using HomeLedger.Infrastructure.Data.Interfaces.Categories;
using HomeLedger.Infrastructure.Entities;

namespace HomeLedger.Infrastructure.Data.Repositories;

public sealed class CategoryRepository(
    HomeLedgerDbContext db,
    ICategoryData data,
    IMapper mapper) : ICategoryRepository
{
    public async Task<IReadOnlyList<Category>> GetAllAsync(CancellationToken ct)
    {
        var entities = await data.GetAllAsync(ct);
        return mapper.Map<IReadOnlyList<Category>>(entities);
    }

    public async Task<Category?> GetAsync(int id, CancellationToken ct)
    {
        var entity = await data.GetAsync(id, ct);
        return entity is null ? null : mapper.Map<Category>(entity);
    }

    public async Task<Category> InsertAsync(Category model, CancellationToken ct)
    {
        await using var transaction = await db.Database.BeginTransactionAsync(ct);

        try
        {
            var entity = mapper.Map<CategoryEntity>(model);
            var insertedEntity = await data.InsertAsync(entity, ct);
            await transaction.CommitAsync(ct);

            return mapper.Map<Category>(insertedEntity);
        }
        catch
        {
            await transaction.RollbackAsync(ct);
            throw;
        }
    }

    public async Task<bool> UpdateAsync(int id, Category model, CancellationToken ct)
    {
        await using var transaction = await db.Database.BeginTransactionAsync(ct);

        try
        {
            model.Id = id;
            var entity = mapper.Map<CategoryEntity>(model);
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
