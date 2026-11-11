using HomeLedger.Domain.BusinessModels;

namespace HomeLedger.Domain.Interfaces.Application;

public interface ICategoryUseCase
{
    Task<IReadOnlyList<Category>> GetAllAsync(CancellationToken ct);

    Task<Category?> GetAsync(int id, CancellationToken ct);

    Task<Category> InsertAsync(Category model, CancellationToken ct);

    Task<bool> UpdateAsync(int id, Category model, CancellationToken ct);

    Task<bool> DeleteAsync(int id, CancellationToken ct);
}
