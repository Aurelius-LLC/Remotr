
namespace Remotr.Example.Calculator;


[RemotrGen]
public class GetValue2Type : StatelessQueryHandler<ICalculatorManagerGrain, double>
{
    public override Task<double> Execute()
    {
        return QueryFactory.GetChild<CalculatorState>()
            .GetValueState2Type()
            .Run(GetPrimaryKey().ToString());
    }
}