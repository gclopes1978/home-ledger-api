namespace HomeLedger.Domain.BusinessModels;

public sealed class Location
{
    public int Id { get; set; }

    public required string Name { get; set; }

    public string? Description { get; set; }
}
