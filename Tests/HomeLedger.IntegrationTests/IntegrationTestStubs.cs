using HomeLedger.Infrastructure.Entities;

namespace HomeLedger.IntegrationTests;

internal static class CategoryStub
{
    public static CategoryEntity Create()
    {
        return new CategoryEntity
        {
            Id = 1,
            Name = "Groceries",
            GroupingId = 1
        };
    }
}

internal static class GroupingStub
{
    public static GroupingEntity Create()
    {
        return new GroupingEntity
        {
            Id = 1,
            Name = "Home"
        };
    }
}

internal static class SourceStub
{
    public static SourceEntity Create()
    {
        return new SourceEntity
        {
            Id = 1,
            Name = "Local Market",
            Description = "Integration test source"
        };
    }
}

internal static class ExpenseStub
{
    public static ExpenseEntity Create(int id = 1, DateOnly? date = null)
    {
        return new ExpenseEntity
        {
            Id = id,
            Date = date ?? new DateOnly(2026, 1, 1),
            SourceId = 1,
            CategoryId = 1,
            PaymentTypeId = 1,
            PurchaseAmount = 25m,
            DiscountAmount = 5m,
            Total = 20m
        };
    }
}

internal static class IncomeStub
{
    public static IncomeEntity Create(int id = 1, DateOnly? date = null)
    {
        return new IncomeEntity
        {
            Id = id,
            Date = date ?? new DateOnly(2026, 1, 1),
            Description = "Monthly salary",
            Amount = 2500m
        };
    }
}

internal static class PaymentTypeStub
{
    public static PaymentTypeEntity Create()
    {
        return new PaymentTypeEntity
        {
            Id = 1,
            Description = "Debit card"
        };
    }
}
