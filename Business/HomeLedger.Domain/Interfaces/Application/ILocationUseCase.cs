using HomeLedger.Domain.BusinessModels;

namespace HomeLedger.Domain.Interfaces.Application;

public interface ILocationUseCase
{
    Task<IReadOnlyList<Location>> GetAllAsync(CancellationToken ct);

    Task<Location?> GetAsync(int id, CancellationToken ct);

    Task<Location> InsertAsync(Location model, CancellationToken ct);

    Task<bool> UpdateAsync(int id, Location model, CancellationToken ct);

    Task<bool> DeleteAsync(int id, CancellationToken ct);
}
