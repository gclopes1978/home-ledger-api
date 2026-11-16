namespace HomeLedger.Domain.BusinessModels;

public sealed class Category
{
    public int Id { get; set; }

    public required string Name { get; set; }

    public int GroupingId { get; set; }

    public Grouping? Grouping { get; set; }
}