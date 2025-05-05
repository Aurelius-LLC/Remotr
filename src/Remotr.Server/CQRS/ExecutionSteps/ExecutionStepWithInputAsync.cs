namespace Remotr;

[GenerateSerializer]
public abstract class ExecutionStepWithInputAsync<Input, Output> : ITakeCqCreator
{
    bool hasRun = false;
    Output? output;

    public virtual void PassCqCreator(ICqCreator creator) { }

    public abstract ValueTask<Output> ExecuteStep(Input input, bool useCache = true);

    // This checks for a cached output from a previous execution to prevent duplicate executions.
    public async ValueTask<Output> Run(Input input, bool useCache = true) {
        if (hasRun && useCache) {
            return output!;
        }
        else {
            output = await ExecuteStep(input, useCache);
            hasRun = true;
            return output;
        }
    }

}
