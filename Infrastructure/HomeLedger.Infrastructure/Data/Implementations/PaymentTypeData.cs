using HomeLedger.Infrastructure.Data.Factories;
using HomeLedger.Infrastructure.Data.Interfaces.PaymentTypes;
using HomeLedger.Infrastructure.Entities;

namespace HomeLedger.Infrastructure.Data.Implementations;

public sealed class PaymentTypeData(HomeLedgerDbContext db)
    : DataFactory<PaymentTypeEntity>(db), IPaymentTypeData
{
}
