
namespace Remotr.Example.Calculator;

public class MultiplyState : StatefulCommandHandler<CalculatorState, double, double>
{
    public override async Task<double> Execute(double input)
    {
        await UpdateState(
            new() {
                Value = (await GetState()).Value * input 
            }
        );
        return input;
    }
}