namespace Remotr;

[GenerateSerializer]
public abstract class ExecutionStepWithInput<Input, Output> : ITakeCqCreator
{
    Output? output;

    public virtual void PassCqCreator(ICqCreator creator) { }

    public abstract Output ExecuteStep(Input input);

    // This checks for a cached output from a previous execution to prevent duplicate executions.
    public Output Run(Input input) {
        if (output != null) {
            return output;
        }
        else {
            output = ExecuteStep(input);
            return output;
        }
    }


}
