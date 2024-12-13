namespace Remotr;

internal sealed record CachedItem
{
    public dynamic? CachedItemForRead { get; set; }

    public CachedItemForTransaction? CachedItemForTransaction { get; set; }
}

internal sealed record CachedItemForTransaction
{
    public DateTime TransactionTimestamp { get; set; }

    public Guid TransactionId { get; set; }

    public dynamic? Item { get; set; }

    public bool IsDeleted { get; set; }
}
