namespace HomeLedger.ApiContract.DTOs;

public sealed record LocationDto(
    int Id,
    string Name,
    string? Description);
