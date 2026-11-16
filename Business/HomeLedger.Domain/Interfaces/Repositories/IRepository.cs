namespace HomeLedger.Domain.Interfaces.Repositories;

public interface IRepository<TModel>
    where TModel : class
{
    Task<IReadOnlyList<TModel>> GetAllAsync(CancellationToken ct);

    Task<TModel?> GetAsync(int id, CancellationToken ct);

    Task<TModel> InsertAsync(TModel model, CancellationToken ct);

    Task<bool> UpdateAsync(int id, TModel model, CancellationToken ct);

    Task<bool> DeleteAsync(int id, CancellationToken ct);
}
