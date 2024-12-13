namespace Remotr;

[GenerateSerializer]
public class SimpleStepWithInputAsyncWrapper<From, Input, To> : ExecutionStep<To>
{

    [Id(0)]
    public ExecutionStep<From>? PreviousStep { get; init; }

    [Id(1)]
    public ExecutionStepWithInputAsync<Input, To> CurrentStep { get; init; }

    [Id(2)]
    public Input StepInput { get; init; }

    public override void PassCqCreator(ICqCreator creator)
    {
        if (PreviousStep != null)
        {
            PreviousStep.PassCqCreator(creator);
        }
        CurrentStep.PassCqCreator(creator);
    }

    public override async ValueTask<To> Run()
    {
        if (PreviousStep != null)
        {
            await PreviousStep.Run();
        }
        return await CurrentStep.Run(StepInput);
    }
}
