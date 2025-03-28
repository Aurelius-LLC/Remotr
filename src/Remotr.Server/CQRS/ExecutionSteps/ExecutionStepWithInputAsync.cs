namespace Remotr;

[GenerateSerializer]
public abstract class ExecutionStepWithInputAsync<Input, Output> : ITakeCqCreator
{
    Output? output;

    public virtual void PassCqCreator(ICqCreator creator) { }

    public abstract ValueTask<Output> ExecuteStep(Input input);

    // This checks for a cached output from a previous execution to prevent duplicate executions.
    public async ValueTask<Output> Run(Input input) {
        if (output != null) {
            return output;
        }
        else {
            output = await ExecuteStep(input);
            return output;
        }
    }

}
