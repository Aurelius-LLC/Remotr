namespace Remotr;

[GenerateSerializer]
public class MergeStepsWrapper<Input1, Input2, Output> : ExecutionStep<Output>
{
    [Id(0)]
    public ExecutionStep<Input1> Step1 { get; init; } = default!;

    [Id(1)]
    public ExecutionStep<Input2> Step2 { get; init; } = default!;

    [Id(2)]
    public IMergeInputs<Input1, Input2, Output> Merger { get; init; } = default!;


    public override void PassCqCreator(ICqCreator creator)
    {
        Step1.PassCqCreator(creator);
        Step2.PassCqCreator(creator);
    }


    public override async ValueTask<Output> ExecuteStep()
    {
        var result1 = await Step1.Run();
        var result2 = await Step2.Run();
        return Merger.Execute(result1, result2);
    }
}


[GenerateSerializer]
public class MergeStepsWrapper<Merger, Input1, Input2, Output> : ExecutionStep<Output>
    where Merger : IMergeInputs<Input1, Input2, Output>
{
    private Merger? merger;

    [Id(0)]
    public ExecutionStep<Input1> Step1 { get; init; } = default!;

    [Id(1)]
    public ExecutionStep<Input2> Step2 { get; init; } = default!;

    public override void PassCqCreator(ICqCreator creator)
    {
        Step1.PassCqCreator(creator);
        Step2.PassCqCreator(creator);
        if (merger == null) {
            merger = creator.InstantiateMerger<Merger>();
        }
    }


    public override async ValueTask<Output> ExecuteStep()
    {
        var result1 = await Step1.Run();
        var result2 = await Step2.Run();
        return merger!.Execute(result1, result2);
    }
}
