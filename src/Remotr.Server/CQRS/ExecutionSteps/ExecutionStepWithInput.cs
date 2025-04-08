namespace Remotr;

[GenerateSerializer]
public abstract class ExecutionStepWithInput<Input, Output> : ITakeCqCreator
{
    bool hasRun = false;
    Output? output;

    public virtual void PassCqCreator(ICqCreator creator) { }

    public abstract Output ExecuteStep(Input input, bool useCache = true);

    // This checks for a cached output from a previous execution to prevent duplicate executions.
    public Output Run(Input input, bool useCache = true) {
        if (hasRun && useCache) {
            return output!;
        }
        else {
            output = ExecuteStep(input, useCache);
            hasRun = true;
            return output;
        }
    }


}
