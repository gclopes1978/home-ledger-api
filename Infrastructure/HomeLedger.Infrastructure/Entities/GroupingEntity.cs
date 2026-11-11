namespace HomeLedger.Infrastructure.Entities;

public sealed class GroupingEntity
{
    public int Id { get; set; }

    public required string Name { get; set; }

    public ICollection<CategoryEntity> Categories { get; set; } = new List<CategoryEntity>();
}
