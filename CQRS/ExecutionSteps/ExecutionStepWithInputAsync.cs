namespace Remotr;

[GenerateSerializer]
public abstract class ExecutionStepWithInputAsync<Input, Output> : ITakeCqCreator
{
    public virtual void PassCqCreator(ICqCreator creator) { }
    public abstract ValueTask<Output> Run(Input input);
}
