

namespace Remotr.Samples.Calculator;

[UseShortcuts]
public class GetValueState<T> : EntityQueryHandler<T, double> where T : IContainValue, new()
{
    public override async Task<double> Execute()
    {
        var state = await GetState();
        return state.GetValue();
    }
}