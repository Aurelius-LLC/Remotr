namespace Remotr;

[GenerateSerializer]
public class PropBridgeStepWrapper<From, To> : ExecutionStepWithInputAsync<From, To>
{
    [Id(0)]
    public NeedsPropStep<From> BridgeStep { get; init; } = default!;

    [Id(1)]
    public ExecutionStep<To> CurrentStep { get; init; } = default!;

    public override void PassCqCreator(ICqCreator creator)
    {
        BridgeStep.PassCqCreator(creator);
        CurrentStep.PassCqCreator(creator);
    }

    public override async ValueTask<To> ExecuteStep(From input, bool useCache)
    {
        BridgeStep.Value = input;
        return await CurrentStep.Run(useCache);
    }
}
