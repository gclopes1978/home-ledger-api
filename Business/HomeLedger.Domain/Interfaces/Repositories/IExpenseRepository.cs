using HomeLedger.Domain.BusinessModels;

namespace HomeLedger.Domain.Interfaces.Repositories;

public interface IExpenseRepository : IRepository<Expense>
{
    Task<IReadOnlyList<Expense>> GetAllAsync(
        DateOnly? startDate,
        DateOnly? endDate,
        CancellationToken ct);
}