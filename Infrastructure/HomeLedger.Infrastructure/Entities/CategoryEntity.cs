namespace HomeLedger.Infrastructure.Entities;

public sealed class CategoryEntity
{
    public int Id { get; set; }

    public required string Name { get; set; }

    public int GroupingId { get; set; }

    public GroupingEntity? Grouping { get; set; }
}
