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

    public override IEnumerable<To> Run(IEnumerable<From> input)
    {
        List<To> transforms = new();
        foreach (var i in input)
        {
            transforms.Add(CurrentStep.Run(i));
        }
        return transforms;
    }
}
