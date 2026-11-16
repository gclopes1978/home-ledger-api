namespace HomeLedger.Domain.BusinessModels;

public sealed class Source
{
    public int Id { get; set; }

    public required string Name { get; set; }

    public string? Description { get; set; }
}
