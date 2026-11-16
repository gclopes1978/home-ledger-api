namespace HomeLedger.Infrastructure.Entities;

public sealed class IncomeEntity
{
    public int Id { get; set; }

    public DateOnly Date { get; set; }

    public required string Description { get; set; }

    public decimal Amount { get; set; }
}
