namespace Remotr;

[GenerateSerializer]
public sealed record TransactionOperations
{
    [Id(0)]
    public List<TransactionStateOperation> StateOperations { get; set; } = new();
}
