namespace Remotr;

[GenerateSerializer]
public class ReduceStepWrapper<Reducer, IO> : ExecutionStepWithInput<IEnumerable<IO>, IO>
    where Reducer : IReduceInputs<IO>
{
    private Reducer? reducer;

    public override void PassCqCreator(ICqCreator creator)
    {
        if (reducer == null) {
            reducer = creator.InstantiateReducer<Reducer>();
        }
    }

    public override IO ExecuteStep(IEnumerable<IO> input, bool useCache)
    {

        IO? curr = default;
        foreach (var i in input)
        {
            if (curr == null)
            {
                curr = i;
            }
            else
            {
                curr = reducer!.Execute(curr, i);
            }
        }
        return curr!;
    }
}

[GenerateSerializer]
public class ReduceStepWrapper<IO> : ExecutionStepWithInput<IEnumerable<IO>, IO>
{
    [Id(0)]
    public IReduceInputs<IO> Reducer { get; init; } = default!;

    public override void PassCqCreator(ICqCreator creator) { }

    public override IO ExecuteStep(IEnumerable<IO> input, bool useCache)
    {

        IO? curr = default;
        foreach (var i in input)
        {
            if (curr == null)
            {
                curr = i;
            }
            else
            {
                curr = Reducer.Execute(curr, i);
            }
        }
        return curr!;
    }
}
