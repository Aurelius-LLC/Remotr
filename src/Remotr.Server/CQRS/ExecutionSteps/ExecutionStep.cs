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
    Output? cachedOutput;

    public virtual void PassCqCreator(ICqCreator creator) { }
    
    public abstract ValueTask<Output> ExecuteStep();

    // This checks for a cached output from a previous execution to prevent duplicate executions.
    public async ValueTask<Output> Run() {
        if (cachedOutput != null) {
            return cachedOutput!;
        }
        else {
            cachedOutput = await ExecuteStep();
            return cachedOutput;
        }
    }

}
