namespace Remotr;

public sealed record TransactionMetadata
{
    public Guid TransactionId { get; set; }

    public DateTime TimeStamp { get; set; }
}
