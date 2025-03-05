
namespace Remotr.Example.Calculator;
public class SetValue3Type : StatelessCommandHandler<ICalculatorManagerGrain, int, double>
{
    public override async Task<double> Execute(int input)
    {
        var x = CommandFactory.GetChild<CalculatorState>();
            //.Tell<SetValueState, double, double>(input);
        return await CommandFactory.GetChild<CalculatorState>()
            .SetValueState3Type(input)
            .Run(GetPrimaryKey().ToString());
    }
}

