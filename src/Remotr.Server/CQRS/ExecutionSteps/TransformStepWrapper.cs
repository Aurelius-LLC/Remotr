namespace Remotr;

[GenerateSerializer]
public class TransformStepWrapper<From, To> : ExecutionStep<To>
{
    [Id(0)]
    public required ExecutionStep<From> PreviousStep { get; init; }

    [Id(1)]
    public required ExecutionStepWithInput<From, To> CurrentStep { get; init; }

    public override void PassCqCreator(ICqCreator creator)
    {
        PreviousStep.PassCqCreator(creator);
        CurrentStep.PassCqCreator(creator);
    }

    public override async ValueTask<To> ExecuteStep()
    {
        var input = await PreviousStep.Run();
        return CurrentStep.Run(input);
    }
}
