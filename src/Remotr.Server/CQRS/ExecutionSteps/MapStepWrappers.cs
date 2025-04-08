namespace Remotr;

[GenerateSerializer]
public class MapStepWrapper<Mapper, Input, Output> : ExecutionStepWithInput<Input, Output>
    where Mapper : IMapInput<Input, Output>
{
    private Mapper? mapper = default;

    public override void PassCqCreator(ICqCreator creator)
    {
        if (mapper == null) {
            mapper = creator.InstantiateMapper<Mapper>();
        }
    }

    public override Output ExecuteStep(Input input, bool useCache = true)
    {
        return mapper!.Execute(input);
    }
}

[GenerateSerializer]
public class MapStepWrapper<Input, Output> : ExecutionStepWithInput<Input, Output>
{
    [Id(0)]
    public IMapInput<Input, Output> Mapper { get; init; } = default!;

    public override Output ExecuteStep(Input input, bool useCache = true)
    {
        return Mapper.Execute(input);
    }
}
