using HomeLedger.Domain.BusinessModels;

namespace HomeLedger.Domain.Interfaces.Application;

public interface ISourceUseCase
{
    Task<IReadOnlyList<Source>> GetAllAsync(CancellationToken ct);

    Task<Source?> GetAsync(int id, CancellationToken ct);

    Task<Source> InsertAsync(Source model, CancellationToken ct);

    Task<bool> UpdateAsync(int id, Source model, CancellationToken ct);

    Task<bool> DeleteAsync(int id, CancellationToken ct);
}
