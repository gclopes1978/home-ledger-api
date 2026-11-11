using AutoMapper;
using HomeLedger.Domain.BusinessModels;
using HomeLedger.Domain.Interfaces.Repositories;
using HomeLedger.Infrastructure.Data.Interfaces.Incomes;
using HomeLedger.Infrastructure.Entities;

namespace HomeLedger.Infrastructure.Data.Repositories;

public sealed class IncomeRepository(
    HomeLedgerDbContext db,
    IIncomeData data,
    IMapper mapper) : IIncomeRepository
{
    public Task<IReadOnlyList<Income>> GetAllAsync(CancellationToken ct)
    {
        return GetAllAsync(null, null, ct);
    }

    public async Task<IReadOnlyList<Income>> GetAllAsync(
        DateOnly? startDate,
        DateOnly? endDate,
        CancellationToken ct)
    {
        var entities = await data.GetAllAsync(startDate, endDate, ct);
        return mapper.Map<IReadOnlyList<Income>>(entities);
    }

    public async Task<Income?> GetAsync(int id, CancellationToken ct)
    {
        var entity = await data.GetAsync(id, ct);
        return entity is null ? null : mapper.Map<Income>(entity);
    }

    public async Task<Income> InsertAsync(Income model, CancellationToken ct)
    {
        await using var transaction = await db.Database.BeginTransactionAsync(ct);
        try
        {
            var entity = mapper.Map<IncomeEntity>(model);
            var insertedEntity = await data.InsertAsync(entity, ct);
            await transaction.CommitAsync(ct);
            return mapper.Map<Income>(insertedEntity);
        }
        catch
        {
            await transaction.RollbackAsync(ct);
            throw;
        }
    }

    public async Task<bool> UpdateAsync(int id, Income model, CancellationToken ct)
    {
        await using var transaction = await db.Database.BeginTransactionAsync(ct);
        try
        {
            model.Id = id;
            var entity = mapper.Map<IncomeEntity>(model);
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
