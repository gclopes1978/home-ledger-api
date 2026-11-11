using HomeLedger.Infrastructure.Data.Factories;
using HomeLedger.Infrastructure.Data.Interfaces.Incomes;
using HomeLedger.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace HomeLedger.Infrastructure.Data.Implementations;

public sealed class IncomeData(HomeLedgerDbContext db)
    : DataFactory<IncomeEntity>(db), IIncomeData
{
    public override Task<IReadOnlyList<IncomeEntity>> GetAllAsync(CancellationToken ct)
    {
        return GetAllAsync(null, null, ct);
    }

    public async Task<IReadOnlyList<IncomeEntity>> GetAllAsync(
        DateOnly? startDate,
        DateOnly? endDate,
        CancellationToken ct)
    {
        var query = Entities.AsNoTracking();

        if (startDate.HasValue)
        {
            query = query.Where(income => income.Date >= startDate.Value);
        }

        if (endDate.HasValue)
        {
            query = query.Where(income => income.Date <= endDate.Value);
        }

        return await query.ToListAsync(ct);
    }
}