
namespace Remotr.Example.Calculator;


[RemotrGen]
public class GetValue2Type2 : StatelessQueryHandler<ICalculatorManagerGrain, double>
{
    public override Task<double> Execute()
    {
        return QueryFactory.GetChild<CalculatorState>()
            .GetValueState3Type(3)
            .Run(GetPrimaryKey().ToString());
    }
}