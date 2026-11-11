using HomeLedger.Infrastructure.Data.Factories;
using HomeLedger.Infrastructure.Data.Interfaces.Expenses;
using HomeLedger.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace HomeLedger.Infrastructure.Data.Implementations;

public sealed class ExpenseData(HomeLedgerDbContext db)
    : DataFactory<ExpenseEntity>(db), IExpenseData
{
    public override Task<IReadOnlyList<ExpenseEntity>> GetAllAsync(CancellationToken ct)
    {
        return GetAllAsync(null, null, ct);
    }

    public async Task<IReadOnlyList<ExpenseEntity>> GetAllAsync(
        DateOnly? startDate,
        DateOnly? endDate,
        CancellationToken ct)
    {
        var query = QueryWithRelatedEntities();

        if (startDate.HasValue)
        {
            query = query.Where(expense => expense.Date >= startDate.Value);
        }

        if (endDate.HasValue)
        {
            query = query.Where(expense => expense.Date <= endDate.Value);
        }

        return await query.ToListAsync(ct);
    }

    public override async Task<ExpenseEntity?> GetAsync(int id, CancellationToken ct)
    {
        return await QueryWithRelatedEntities()
            .FirstOrDefaultAsync(expense => expense.Id == id, ct);
    }

    public override async Task<ExpenseEntity> InsertAsync(ExpenseEntity entity, CancellationToken ct)
    {
        var inserted = await base.InsertAsync(entity, ct);
        return await GetAsync(inserted.Id, ct) ?? inserted;
    }

    private IQueryable<ExpenseEntity> QueryWithRelatedEntities()
    {
        return Entities
            .AsNoTracking()
            .Include(expense => expense.Category)
                .ThenInclude(category => category!.Grouping)
            .Include(expense => expense.Location)
            .Include(expense => expense.PaymentType);
    }
}