namespace HomeLedger.ApiContract.DTOs;

public sealed record ExpenseDto(
    int Id,
    DateOnly Date,
    int SourceId,
    int CategoryId,
    int PaymentTypeId,
    decimal PurchaseAmount,
    decimal DiscountAmount,
    decimal Total,
    CategoryDto? Category = null,
    SourceDto? Source = null,
    PaymentTypeDto? PaymentType = null);