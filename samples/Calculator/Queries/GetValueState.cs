

namespace Remotr.Samples.Calculator;

[RemotrGen]
public class GetValueState<T> : StatefulQueryHandler<T, double> where T : IContainValue, new()
{
    public override async Task<double> Execute()
    {
        var state = await GetState();
        return state.GetValue();
    }
}