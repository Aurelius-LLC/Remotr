namespace Remotr;

[GenerateSerializer]
public sealed record TransactionStateOperation
{
    [Id(0)]
    public TransactionStateOperationType TransactionOperationType { get; set; }

    [Id(1)]
    public string? ItemId { get; set; }

    [Id(2)]
    public string? ItemJson { get; set; }
}
