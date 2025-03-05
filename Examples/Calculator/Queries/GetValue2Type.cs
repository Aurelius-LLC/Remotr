
namespace Remotr.Example.Calculator;


[RemotrGen]
public class GetValue2Type : StatelessQueryHandler<ICalculatorManagerGrain, double>
{
    public override Task<double> Execute()
    {
        return QueryFactory.GetChild<CalculatorState>()
            .Ask<GetValueState2Type, double>()
            .Run(GetPrimaryKey().ToString());
    }
}