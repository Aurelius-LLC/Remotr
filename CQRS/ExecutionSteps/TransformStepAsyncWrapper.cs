namespace Remotr;

[GenerateSerializer]
public class TransformStepAsyncWrapper<From, To> : ExecutionStep<To>
{

    [Id(0)]
    public ExecutionStep<From> PreviousStep { get; init; }

    [Id(1)]
    public ExecutionStepWithInputAsync<From, To> CurrentStep { get; init; }

    public override void PassCqCreator(ICqCreator creator)
    {
        PreviousStep.PassCqCreator(creator);
        CurrentStep.PassCqCreator(creator);
    }

    public override async ValueTask<To> Run()
    {
        var input = await PreviousStep.Run();
        return await CurrentStep.Run(input);
    }
}
