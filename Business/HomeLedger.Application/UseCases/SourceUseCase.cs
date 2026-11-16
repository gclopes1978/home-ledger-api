using HomeLedger.Domain.BusinessModels;
using HomeLedger.Domain.Interfaces.Application;
using HomeLedger.Domain.Interfaces.Repositories;

namespace HomeLedger.Application.UseCases;

public sealed class SourceUseCase(ISourceRepository repository) : ISourceUseCase
{
    public Task<IReadOnlyList<Source>> GetAllAsync(CancellationToken ct)
    {
        return repository.GetAllAsync(ct);
    }

    public Task<Source?> GetAsync(int id, CancellationToken ct)
    {
        return repository.GetAsync(id, ct);
    }

    public Task<Source> InsertAsync(Source model, CancellationToken ct)
    {
        return repository.InsertAsync(model, ct);
    }

    public Task<bool> UpdateAsync(int id, Source model, CancellationToken ct)
    {
        return repository.UpdateAsync(id, model, ct);
    }

    public Task<bool> DeleteAsync(int id, CancellationToken ct)
    {
        return repository.DeleteAsync(id, ct);
    }
}
