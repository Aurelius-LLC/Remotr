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
    bool hasRun = false;
    Output? cachedOutput;

    public virtual void PassCqCreator(ICqCreator creator) { }
    
    public abstract ValueTask<Output> ExecuteStep(bool useCache = true);

    // This checks for a cached output from a previous execution to prevent duplicate executions.
    public async ValueTask<Output> Run(bool useCache = true) {
        if (hasRun && useCache) {
            return cachedOutput!;
        }
        else {
            cachedOutput = await ExecuteStep(useCache);
            hasRun = true;
            return cachedOutput;
        }
    }

}
