namespace HomeLedger.ApiContract.DTOs;

public sealed record SourceDto(
    int Id,
    string Name,
    string? Description);
