using HomeLedger.Domain.BusinessModels;

namespace HomeLedger.Domain.Interfaces.Repositories;

public interface IIncomeRepository : IRepository<Income>
{
    Task<IReadOnlyList<Income>> GetAllAsync(
        DateOnly? startDate,
        DateOnly? endDate,
        CancellationToken ct);
}