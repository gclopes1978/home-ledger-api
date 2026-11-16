using HomeLedger.Domain.BusinessModels;
using HomeLedger.Domain.Interfaces.Application;
using HomeLedger.Domain.Interfaces.Repositories;

namespace HomeLedger.Application.UseCases;

public sealed class GroupingUseCase(IGroupingRepository repository) : IGroupingUseCase
{
    public Task<IReadOnlyList<Grouping>> GetAllAsync(CancellationToken ct)
    {
        return repository.GetAllAsync(ct);
    }

    public Task<Grouping?> GetAsync(int id, CancellationToken ct)
    {
        return repository.GetAsync(id, ct);
    }

    public Task<Grouping> InsertAsync(Grouping model, CancellationToken ct)
    {
        return repository.InsertAsync(model, ct);
    }

    public Task<bool> UpdateAsync(int id, Grouping model, CancellationToken ct)
    {
        return repository.UpdateAsync(id, model, ct);
    }

    public Task<bool> DeleteAsync(int id, CancellationToken ct)
    {
        return repository.DeleteAsync(id, ct);
    }
}
