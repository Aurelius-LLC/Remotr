
namespace Remotr.Example.Calculator;
public class Multiply : StatelessCommandHandler<ICalculatorManagerGrain, double, double>
{
    public override async Task<double> Execute(double input)
    {
        return await CommandFactory.GetChild<CalculatorState>()
            .Tell<MultiplyState, double, double>(input)
            .Run(GetPrimaryKey().ToString());
    }
}