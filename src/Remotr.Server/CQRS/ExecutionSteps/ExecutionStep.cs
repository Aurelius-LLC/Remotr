namespace Remotr;

[GenerateSerializer]
public abstract class ExecutionStepNameWrapper
{
    [Id(0)]
    public string Name { get; set; } = "";
}

[GenerateSerializer]
public abstract class ExecutionStep<Output> : ExecutionStepNameWrapper, ITakeCqCreator
{
    public virtual void PassCqCreator(ICqCreator creator) { }
    public abstract ValueTask<Output> Run();
}
