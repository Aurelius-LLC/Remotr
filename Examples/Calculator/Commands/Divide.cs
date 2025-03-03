

namespace Remotr.Example.Calculator;
public class Divide : StatelessCommandHandler<ICalculatorManagerGrain, double, double>
{
    public override async Task<double> Execute(double input)
    {
        return await CommandFactory.GetChild<CalculatorState>()
            .Tell<DivideState, double, double>(input)
            .Run(GetPrimaryKey().ToString());
    }
}