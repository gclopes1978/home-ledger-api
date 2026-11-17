using HomeLedger.Infrastructure.Entities;

namespace HomeLedger.IntegrationTests.Stubs;

internal static class GroupingStub
{
    public static GroupingEntity Create()
    {
        return new GroupingEntity
        {
            Name = "Home"
        };
    }
}
