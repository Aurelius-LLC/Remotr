namespace Remotr;

[GenerateSerializer]
public class EmptyStep : ExecutionStep<object>
{
    public override ValueTask<object> Run()
    {
        return ValueTask.FromResult(new object());
    }
}
