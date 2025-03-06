
namespace Remotr.Example.Calculator;

[RemotrGen]
public class Multiply : StatelessCommandHandler<ICalculatorManagerGrain, double, double>
{
    public override async Task<double> Execute(double input)
    {
        return await CommandFactory.GetChild<CalculatorState>()
            .MultiplyState(input)
            .Run(GetPrimaryKey().ToString());
    }
}