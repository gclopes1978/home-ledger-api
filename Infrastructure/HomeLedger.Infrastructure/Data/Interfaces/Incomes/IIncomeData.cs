using HomeLedger.Infrastructure.Entities;

namespace HomeLedger.Infrastructure.Data.Interfaces.Incomes;

public interface IIncomeData : IData<IncomeEntity>
{
    Task<IReadOnlyList<IncomeEntity>> GetAllAsync(
        DateOnly? startDate,
        DateOnly? endDate,
        CancellationToken ct);
}