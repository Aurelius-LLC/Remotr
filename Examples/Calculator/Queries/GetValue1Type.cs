
namespace Remotr.Example.Calculator;

public class GetValue1Type : StatelessQueryHandler<ICalculatorManagerGrain, double>
{
    public override Task<double> Execute()
    {
        return QueryFactory.GetChild<CalculatorState>()
            .Ask<GetValueState1Type, double>()
            .Run(GetPrimaryKey().ToString());
    }
}