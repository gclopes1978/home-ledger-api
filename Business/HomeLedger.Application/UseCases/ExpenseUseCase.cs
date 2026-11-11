using HomeLedger.Domain.BusinessModels;
using HomeLedger.Domain.Interfaces.Application;
using HomeLedger.Domain.Interfaces.Repositories;

namespace HomeLedger.Application.UseCases;

public sealed class ExpenseUseCase(IExpenseRepository repository) : IExpenseUseCase
{
    public Task<IReadOnlyList<Expense>> GetAllAsync(DateOnly? startDate, DateOnly? endDate, CancellationToken ct)
    {
        return repository.GetAllAsync(startDate, endDate, ct);
    }

    public Task<Expense?> GetAsync(int id, CancellationToken ct)
    {
        return repository.GetAsync(id, ct);
    }

    public Task<Expense> InsertAsync(Expense model, CancellationToken ct)
    {
        return repository.InsertAsync(model, ct);
    }

    public Task<bool> UpdateAsync(int id, Expense model, CancellationToken ct)
    {
        return repository.UpdateAsync(id, model, ct);
    }

    public Task<bool> DeleteAsync(int id, CancellationToken ct)
    {
        return repository.DeleteAsync(id, ct);
    }
}