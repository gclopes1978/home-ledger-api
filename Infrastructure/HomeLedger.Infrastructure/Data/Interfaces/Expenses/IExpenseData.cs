using HomeLedger.Infrastructure.Entities;

namespace HomeLedger.Infrastructure.Data.Interfaces.Expenses;

public interface IExpenseData : IData<ExpenseEntity>
{
    Task<IReadOnlyList<ExpenseEntity>> GetAllAsync(
        DateOnly? startDate,
        DateOnly? endDate,
        CancellationToken ct);
}