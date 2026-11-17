using HomeLedger.Infrastructure.Entities;

namespace HomeLedger.IntegrationTests.Stubs;

internal static class PaymentTypeStub
{
    public static PaymentTypeEntity Create()
    {
        return new PaymentTypeEntity
        {
            Description = "Debit card"
        };
    }
}
