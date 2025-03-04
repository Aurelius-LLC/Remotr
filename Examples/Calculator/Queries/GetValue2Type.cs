
namespace Remotr.Example.Calculator;

public class GetValue2Type : StatelessQueryHandler<ICalculatorManagerGrain, int, double>
{
    public override Task<double> Execute(int input)
    {
        return QueryFactory.GetChild<CalculatorState>()
            .Ask<GetValueState1Type, double>()
            .Run(GetPrimaryKey().ToString());
    }
}