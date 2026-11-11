using HomeLedger.Infrastructure.Data.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HomeLedger.Infrastructure.Data.Factories;

public abstract class DataFactory<TEntity>(HomeLedgerDbContext db) : IData<TEntity>
    where TEntity : class
{
    protected DbSet<TEntity> Entities
    {
        get
        {
            return db.Set<TEntity>();
        }
    }

    public virtual async Task<IReadOnlyList<TEntity>> GetAllAsync(CancellationToken ct)
    {
        return await Entities
            .AsNoTracking()
            .ToListAsync(ct);
    }

    public virtual async Task<TEntity?> GetAsync(int id, CancellationToken ct)
    {
        return await Entities
            .AsNoTracking()
            .FirstOrDefaultAsync(entity => EF.Property<int>(entity, "Id") == id, ct);
    }

    public virtual async Task<TEntity> InsertAsync(TEntity entity, CancellationToken ct)
    {
        Entities.Add(entity);
        await db.SaveChangesAsync(ct);

        return entity;
    }

    public async Task<bool> UpdateAsync(int id, TEntity entity, CancellationToken ct)
    {
        var currentEntity = await Entities.FindAsync([id], ct);
        if (currentEntity is null)
        {
            return false;
        }

        db.Entry(currentEntity).CurrentValues.SetValues(entity);
        await db.SaveChangesAsync(ct);

        return true;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct)
    {
        var entity = await Entities.FindAsync([id], ct);
        if (entity is null)
        {
            return false;
        }

        Entities.Remove(entity);
        await db.SaveChangesAsync(ct);

        return true;
    }
}
