namespace Remotr;

[GenerateSerializer]
public class FilterStepWrapper<Mapper, Input, Output> : ExecutionStepWithInput<Input, IEnumerable<Output>>
    where Input : IEnumerable<Output>
    where Mapper : IMapInput<Output, bool>
{
    [Id(0)]
    public required MapStepWrapper<Mapper, Output, bool> CurrentStep { get; init; }

    public override void PassCqCreator(ICqCreator creator)
    {
        CurrentStep.PassCqCreator(creator);
    }

    public override IEnumerable<Output> ExecuteStep(Input input)
    {
        List<Output> items = new();
        foreach (var i in input)
        {
            if (CurrentStep.Run(i))
            {
                items.Add(i);
            }
        }
        return items;
    }
}

[GenerateSerializer]
public class FilterStepWrapper<Input, Output> : ExecutionStepWithInput<Input, IEnumerable<Output>>
    where Input : IEnumerable<Output>
{
    [Id(0)]
    public required MapStepWrapper<Output, bool> CurrentStep { get; init; }

    public override void PassCqCreator(ICqCreator creator)
    {
        CurrentStep.PassCqCreator(creator);
    }

    public override IEnumerable<Output> ExecuteStep(Input input)
    {
        List<Output> items = new();
        foreach (var i in input)
        {
            if (CurrentStep.Run(i))
            {
                items.Add(i);
            }
        }
        return items;
    }
}
