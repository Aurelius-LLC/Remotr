namespace Remotr;

[GenerateSerializer]
public class LoopStepAsyncWrapper<From, To> : ExecutionStepWithInputAsync<IEnumerable<From>, IEnumerable<To>>
{
    [Id(0)]
    public required ExecutionStepWithInputAsync<From, To> CurrentStep { get; init; }

    public override void PassCqCreator(ICqCreator creator)
    {
        CurrentStep.PassCqCreator(creator);
    }

    public override async ValueTask<IEnumerable<To>> ExecuteStep(IEnumerable<From> input)
    {
        List<To> transforms = new();
        foreach (var i in input)
        {
            transforms.Add(await CurrentStep.Run(i));
        }
        return transforms;
    }
}
