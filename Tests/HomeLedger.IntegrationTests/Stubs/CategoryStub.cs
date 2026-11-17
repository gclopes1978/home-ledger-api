using HomeLedger.Infrastructure.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace HomeLedger.IntegrationTests.Stubs;

internal static class CategoryStub
{
    public static CategoryEntity Create(GroupingEntity grouping)
    {
        return new CategoryEntity
        {
            Name = "Groceries",
            Grouping = grouping
        };
    }
}
