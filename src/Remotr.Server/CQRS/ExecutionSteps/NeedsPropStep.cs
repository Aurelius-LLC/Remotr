namespace Remotr;

[GenerateSerializer]
public class NeedsPropStep<T> : ExecutionStep<T>
{
    [Id(0)]
    public T? Value { get; set; }

    public override ValueTask<T> ExecuteStep()
    {
        return ValueTask.FromResult(Value!);
    }
}
