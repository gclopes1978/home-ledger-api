using HomeLedger.Domain.BusinessModels;
using HomeLedger.Domain.Interfaces.Application;
using HomeLedger.Domain.Interfaces.Repositories;

namespace HomeLedger.Application.UseCases;

public sealed class CategoryUseCase(ICategoryRepository repository) : ICategoryUseCase
{
    public Task<IReadOnlyList<Category>> GetAllAsync(CancellationToken ct)
    {
        return repository.GetAllAsync(ct);
    }

    public Task<Category?> GetAsync(int id, CancellationToken ct)
    {
        return repository.GetAsync(id, ct);
    }

    public Task<Category> InsertAsync(Category model, CancellationToken ct)
    {
        return repository.InsertAsync(model, ct);
    }

    public Task<bool> UpdateAsync(int id, Category model, CancellationToken ct)
    {
        return repository.UpdateAsync(id, model, ct);
    }

    public Task<bool> DeleteAsync(int id, CancellationToken ct)
    {
        return repository.DeleteAsync(id, ct);
    }
}
