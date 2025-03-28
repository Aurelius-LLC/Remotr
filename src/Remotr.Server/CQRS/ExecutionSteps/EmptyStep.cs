namespace Remotr;

[GenerateSerializer]
public class EmptyStep : ExecutionStep<object>
{
    public override ValueTask<object> ExecuteStep()
    {
        return ValueTask.FromResult(new object());
    }
}
