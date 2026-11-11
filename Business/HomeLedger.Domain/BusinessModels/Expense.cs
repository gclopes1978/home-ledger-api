namespace HomeLedger.Domain.BusinessModels;

public sealed class Expense
{
    public int Id { get; set; }

    public DateOnly Date { get; set; }

    public int LocationId { get; set; }

    public int CategoryId { get; set; }

    public int PaymentTypeId { get; set; }

    public decimal PurchaseAmount { get; set; }

    public decimal DiscountAmount { get; set; }

    public decimal Total { get; set; }

    public Category? Category { get; set; }

    public Location? Location { get; set; }

    public PaymentType? PaymentType { get; set; }
}