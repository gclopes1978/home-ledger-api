namespace HomeLedger.Infrastructure.Entities;

public sealed class SourceEntity
{
    public int Id { get; set; }

    public required string Name { get; set; }

    public string? Description { get; set; }
}
