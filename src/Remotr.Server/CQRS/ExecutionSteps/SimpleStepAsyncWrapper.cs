namespace Remotr;

[GenerateSerializer]
public class SimpleStepAsyncWrapper<From, To> : ExecutionStep<To>
{

    [Id(0)]
    public ExecutionStep<From>? PreviousStep { get; init; }

    [Id(1)]
    public ExecutionStep<To> CurrentStep { get; init; } = default!;

    public override void PassCqCreator(ICqCreator creator)
    {
        if (PreviousStep != null)
        {
            PreviousStep.PassCqCreator(creator);
        }
        CurrentStep.PassCqCreator(creator);
    }

    public override async ValueTask<To> ExecuteStep()
    {
        if (PreviousStep != null)
        {
            await PreviousStep.Run();
        }
        return await CurrentStep.Run();
    }
}
