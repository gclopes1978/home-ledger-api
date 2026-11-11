namespace HomeLedger.Infrastructure.Entities;

public sealed class ExpenseEntity
{
    public int Id { get; set; }

    public DateOnly Date { get; set; }

    public int LocationId { get; set; }

    public int CategoryId { get; set; }

    public int PaymentTypeId { get; set; }

    public decimal PurchaseAmount { get; set; }

    public decimal DiscountAmount { get; set; }

    public decimal Total { get; set; }

    public LocationEntity? Location { get; set; }

    public CategoryEntity? Category { get; set; }

    public PaymentTypeEntity? PaymentType { get; set; }
}