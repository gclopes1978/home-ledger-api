using HomeLedger.Domain.BusinessModels;
using HomeLedger.Domain.Interfaces.Application;
using HomeLedger.Domain.Interfaces.Repositories;

namespace HomeLedger.Application.UseCases;

public sealed class PaymentTypeUseCase(IPaymentTypeRepository repository) : IPaymentTypeUseCase
{
    public Task<IReadOnlyList<PaymentType>> GetAllAsync(CancellationToken ct)
    {
        return repository.GetAllAsync(ct);
    }

    public Task<PaymentType?> GetAsync(int id, CancellationToken ct)
    {
        return repository.GetAsync(id, ct);
    }

    public Task<PaymentType> InsertAsync(PaymentType model, CancellationToken ct)
    {
        return repository.InsertAsync(model, ct);
    }

    public Task<bool> UpdateAsync(int id, PaymentType model, CancellationToken ct)
    {
        return repository.UpdateAsync(id, model, ct);
    }

    public Task<bool> DeleteAsync(int id, CancellationToken ct)
    {
        return repository.DeleteAsync(id, ct);
    }
}
