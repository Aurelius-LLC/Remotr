namespace Remotr;

[GenerateSerializer]
public class TransformStepAsyncWrapper<From, To> : ExecutionStep<To>
{

    [Id(0)]
    public required ExecutionStep<From> PreviousStep { get; init; }

    [Id(1)]
    public required  ExecutionStepWithInputAsync<From, To> CurrentStep { get; init; }

    public override void PassCqCreator(ICqCreator creator)
    {
        PreviousStep.PassCqCreator(creator);
        CurrentStep.PassCqCreator(creator);
    }

    public override async ValueTask<To> ExecuteStep(bool useCache)
    {
        var input = await PreviousStep.Run(useCache);
        return await CurrentStep.Run(input, useCache);
    }
}
