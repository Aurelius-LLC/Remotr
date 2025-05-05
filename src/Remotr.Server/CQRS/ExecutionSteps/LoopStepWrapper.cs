namespace Remotr;

[GenerateSerializer]
public class LoopStepWrapper<From, To> : ExecutionStepWithInput<IEnumerable<From>, IEnumerable<To>>
{
    [Id(0)]
    public required ExecutionStepWithInput<From, To> CurrentStep { get; init; }

    public override void PassCqCreator(ICqCreator creator)
    {
        CurrentStep.PassCqCreator(creator);
    }

    public override IEnumerable<To> ExecuteStep(IEnumerable<From> input, bool useCache = true)
    {
        List<To> transforms = new();
        foreach (var i in input)
        {
            // Ignore useCache value, as this should set a precedent to not use the cache.
            transforms.Add(CurrentStep.Run(i, false));
        }
        return transforms;
    }
}
