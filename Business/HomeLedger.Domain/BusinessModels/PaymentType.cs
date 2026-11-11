namespace HomeLedger.Domain.BusinessModels;

public sealed class PaymentType
{
    public int Id { get; set; }

    public required string Description { get; set; }
}
