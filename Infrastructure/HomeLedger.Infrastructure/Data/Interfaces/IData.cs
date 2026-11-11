namespace HomeLedger.Infrastructure.Data.Interfaces;

public interface IData<TEntity>
    where TEntity : class
{
    Task<IReadOnlyList<TEntity>> GetAllAsync(CancellationToken ct);

    Task<TEntity?> GetAsync(int id, CancellationToken ct);

    Task<TEntity> InsertAsync(TEntity entity, CancellationToken ct);

    Task<bool> UpdateAsync(int id, TEntity entity, CancellationToken ct);

    Task<bool> DeleteAsync(int id, CancellationToken ct);
}
