namespace HomeLedger.Infrastructure.Entities;

public sealed class PaymentTypeEntity
{
    public int Id { get; set; }

    public required string Description { get; set; }

    public ICollection<ExpenseEntity> Expenses { get; set; } = new List<ExpenseEntity>();
}