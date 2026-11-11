using HomeLedger.Infrastructure.Data.Factories;
using HomeLedger.Infrastructure.Data.Interfaces.Categories;
using HomeLedger.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace HomeLedger.Infrastructure.Data.Implementations;

public sealed class CategoryData(HomeLedgerDbContext db)
    : DataFactory<CategoryEntity>(db), ICategoryData
{
    public override async Task<IReadOnlyList<CategoryEntity>> GetAllAsync(CancellationToken ct)
    {
        return await QueryWithGrouping()
            .ToListAsync(ct);
    }

    public override async Task<CategoryEntity?> GetAsync(int id, CancellationToken ct)
    {
        return await QueryWithGrouping()
            .FirstOrDefaultAsync(category => category.Id == id, ct);
    }

    public override async Task<CategoryEntity> InsertAsync(CategoryEntity entity, CancellationToken ct)
    {
        var inserted = await base.InsertAsync(entity, ct);
        return await GetAsync(inserted.Id, ct) ?? inserted;
    }

    private IQueryable<CategoryEntity> QueryWithGrouping()
    {
        return Entities
            .AsNoTracking()
            .Include(category => category.Grouping);
    }
}