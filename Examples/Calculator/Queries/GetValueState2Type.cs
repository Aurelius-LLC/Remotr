
namespace Remotr.Example.Calculator;

// [RemotrGen]
public class GetValueState2Type<T> : StatefulQueryHandler<CalculatorState, T>
{
    public override Task<T> Execute()
    {
        return Task.FromResult(default(T));
    }
}