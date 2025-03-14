
namespace Remotr.Example.Calculator;

[RemotrGen]
public class MultiplyState : StatefulCommandHandler<CalculatorState, double, double>
{
    public override async Task<double> Execute(double input)
    {
        await UpdateState(
            new() {
                Value = (await GetState()).Value * input 
            }
        );
        return (await GetState()).Value;
    }
}