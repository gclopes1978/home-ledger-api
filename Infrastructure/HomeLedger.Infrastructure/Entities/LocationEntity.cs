namespace HomeLedger.Infrastructure.Entities;

public sealed class LocationEntity
{
    public int Id { get; set; }

    public required string Name { get; set; }

    public string? Description { get; set; }
}
