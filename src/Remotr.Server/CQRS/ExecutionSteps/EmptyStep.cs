namespace Remotr;

[GenerateSerializer]
public class EmptyStep : ExecutionStep<object>
{
    public override ValueTask<object> ExecuteStep(bool useCache = true)
    {
        return ValueTask.FromResult(new object());
    }
}
