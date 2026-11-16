using HomeLedger.Domain.BusinessModels;

namespace HomeLedger.Domain.Interfaces.Application;

public interface IExpenseUseCase
{
    Task<IReadOnlyList<Expense>> GetAllAsync(DateOnly? startDate, DateOnly? endDate, CancellationToken ct);

    Task<Expense?> GetAsync(int id, CancellationToken ct);

    Task<Expense> InsertAsync(Expense model, CancellationToken ct);

    Task<bool> UpdateAsync(int id, Expense model, CancellationToken ct);

    Task<bool> DeleteAsync(int id, CancellationToken ct);
}