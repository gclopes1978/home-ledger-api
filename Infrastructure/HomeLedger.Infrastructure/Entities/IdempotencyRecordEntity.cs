namespace HomeLedger.Infrastructure.Entities;

public sealed class IdempotencyRecordEntity
{
    public int Id { get; set; }

    public required string IdempotencyKey { get; set; }

    public required string RequestHash { get; set; }

    public int? ResponseStatusCode { get; set; }

    public string? ResponseContentType { get; set; }

    public string? ResponseLocation { get; set; }

    public string? ResponseBody { get; set; }

    public DateTimeOffset CreatedAtUtc { get; set; }
}