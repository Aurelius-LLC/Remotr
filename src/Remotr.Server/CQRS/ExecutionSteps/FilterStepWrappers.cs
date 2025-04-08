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

    public override IEnumerable<Output> ExecuteStep(Input input, bool useCache = true)
    {
        List<Output> items = new();
        foreach (var i in input)
        {
            // Ignore useCache value, as this should set a precedent to not use the cache.
            if (CurrentStep.Run(i, false))
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

    public override IEnumerable<Output> ExecuteStep(Input input, bool useCache = true)
    {
        List<Output> items = new();
        foreach (var i in input)
        {
            // Ignore useCache value, as this should set a precedent to not use the cache.
            if (CurrentStep.Run(i, false))
            {
                items.Add(i);
            }
        }
        return items;
    }
}
