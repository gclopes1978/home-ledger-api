using HomeLedger.Domain.BusinessModels;

namespace HomeLedger.Domain.Interfaces.Application;

public interface IPaymentTypeUseCase
{
    Task<IReadOnlyList<PaymentType>> GetAllAsync(CancellationToken ct);

    Task<PaymentType?> GetAsync(int id, CancellationToken ct);

    Task<PaymentType> InsertAsync(PaymentType model, CancellationToken ct);

    Task<bool> UpdateAsync(int id, PaymentType model, CancellationToken ct);

    Task<bool> DeleteAsync(int id, CancellationToken ct);
}
