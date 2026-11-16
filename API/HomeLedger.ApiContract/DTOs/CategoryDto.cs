namespace HomeLedger.ApiContract.DTOs;

public sealed record CategoryDto(
    int Id,
    string Name,
    int GroupingId,
    GroupingDto? Grouping = null);