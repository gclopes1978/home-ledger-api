using HomeLedger.Infrastructure.Entities;

namespace HomeLedger.IntegrationTests.Stubs;

internal static class IncomeStub
{
    public static IncomeEntity Create(DateOnly? date = null)
    {
        return new IncomeEntity
        {
            Date = date ?? new DateOnly(2026, 1, 1),
            Description = "Monthly salary",
            Amount = 2500m
        };
    }
}
