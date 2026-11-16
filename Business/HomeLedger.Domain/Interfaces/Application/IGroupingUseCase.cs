using HomeLedger.Domain.BusinessModels;

namespace HomeLedger.Domain.Interfaces.Application;

public interface IGroupingUseCase
{
    Task<IReadOnlyList<Grouping>> GetAllAsync(CancellationToken ct);

    Task<Grouping?> GetAsync(int id, CancellationToken ct);

    Task<Grouping> InsertAsync(Grouping model, CancellationToken ct);

    Task<bool> UpdateAsync(int id, Grouping model, CancellationToken ct);

    Task<bool> DeleteAsync(int id, CancellationToken ct);
}
