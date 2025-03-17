using Orleans.Runtime;

namespace Remotr;

[GenerateSerializer]
public sealed record ComponentId
{
    [Id(0)]
    public GrainId AggregateId { get; init; }

    [Id(1)]
    public required string ItemId { get; init; }
}
