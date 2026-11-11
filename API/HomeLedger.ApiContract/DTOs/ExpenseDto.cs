namespace HomeLedger.ApiContract.DTOs;

public sealed record ExpenseDto(
    int Id,
    DateOnly Date,
    int LocationId,
    int CategoryId,
    int PaymentTypeId,
    decimal PurchaseAmount,
    decimal DiscountAmount,
    decimal Total,
    CategoryDto? Category = null,
    LocationDto? Location = null,
    PaymentTypeDto? PaymentType = null);