namespace HomeLedger.ApiContract.DTOs;

public sealed record IncomeDto(
    int Id,
    DateOnly Date,
    string Description,
    decimal Amount);
