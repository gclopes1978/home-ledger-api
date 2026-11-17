using HomeLedger.Infrastructure.Entities;

namespace HomeLedger.IntegrationTests.Stubs;

internal static class SourceStub
{
    public static SourceEntity Create()
    {
        return new SourceEntity
        {
            Name = "Local Market",
            Description = "Integration test source"
        };
    }
}
