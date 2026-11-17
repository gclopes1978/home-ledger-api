using HomeLedger.Infrastructure.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace HomeLedger.IntegrationTests.Stubs;

internal static class ExpenseStub
{
    public static ExpenseEntity Create(SourceEntity source, CategoryEntity category, PaymentTypeEntity paymentType,  DateOnly? date = null)
    {
        return new ExpenseEntity
        {
            Date = date ?? new DateOnly(2026, 1, 1),
            Source = source,
            Category = category,
            PaymentType = paymentType,
            PurchaseAmount = 25m,
            DiscountAmount = 5m,
            Total = 20m
        };
    }
}
