namespace Remotr;

[GenerateSerializer]
public abstract class ExecutionStepWithInput<Input, Output> : ITakeCqCreator
{
    public virtual void PassCqCreator(ICqCreator creator) { }
    public abstract Output Run(Input input);
}
