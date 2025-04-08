
namespace Remotr.Samples.Calculator;

[UseShortcuts]
public class MultiplyState : EntityCommandHandler<CalculatorState, double, double>
{
    public override async Task<double> Execute(double input)
    {
        await UpdateState(
            new() {
                Value = (await GetState()).Value * input 
            }
        );
        var value = (await GetState()).Value;
        return value;
    }
}