using HomeLedger.Domain.BusinessModels;
using HomeLedger.Domain.Interfaces.Application;
using HomeLedger.Domain.Interfaces.Repositories;

namespace HomeLedger.Application.UseCases;

public sealed class IncomeUseCase(IIncomeRepository repository) : IIncomeUseCase
{
    public Task<IReadOnlyList<Income>> GetAllAsync(DateOnly? startDate, DateOnly? endDate, CancellationToken ct)
    {
        return repository.GetAllAsync(startDate, endDate, ct);
    }

    public Task<Income?> GetAsync(int id, CancellationToken ct)
    {
        return repository.GetAsync(id, ct);
    }

    public Task<Income> InsertAsync(Income model, CancellationToken ct)
    {
        return repository.InsertAsync(model, ct);
    }

    public Task<bool> UpdateAsync(int id, Income model, CancellationToken ct)
    {
        return repository.UpdateAsync(id, model, ct);
    }

    public Task<bool> DeleteAsync(int id, CancellationToken ct)
    {
        return repository.DeleteAsync(id, ct);
    }
}