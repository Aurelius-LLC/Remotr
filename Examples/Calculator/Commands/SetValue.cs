
namespace Remotr.Example.Calculator;
public class SetValue : StatelessCommandHandler<ICalculatorManagerGrain, double, double>
{
    public override async Task<double> Execute(double input)
    {
        return await CommandFactory.GetChild<CalculatorState>()
            .Tell<SetValueState, double, double>(input)
            .Run(GetPrimaryKey().ToString());
    }
}