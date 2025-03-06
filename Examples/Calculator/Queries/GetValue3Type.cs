
namespace Remotr.Example.Calculator;

[RemotrGen]
public class GetValue3Type : StatelessQueryHandler<ICalculatorManagerGrain, int, double>
{
    public override Task<double> Execute(int input)
    {
        return QueryFactory.GetChild<CalculatorState>()
            .GetValueState2Type()
            .Run(GetPrimaryKey().ToString());
    }
}