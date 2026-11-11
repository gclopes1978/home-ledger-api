using HomeLedger.Domain.BusinessModels;

namespace HomeLedger.Domain.Interfaces.Application;

public interface IIncomeUseCase
{
    Task<IReadOnlyList<Income>> GetAllAsync(DateOnly? startDate, DateOnly? endDate, CancellationToken ct);

    Task<Income?> GetAsync(int id, CancellationToken ct);

    Task<Income> InsertAsync(Income model, CancellationToken ct);

    Task<bool> UpdateAsync(int id, Income model, CancellationToken ct);

    Task<bool> DeleteAsync(int id, CancellationToken ct);
}