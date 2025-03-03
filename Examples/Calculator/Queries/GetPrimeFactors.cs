namespace Remotr.Example.Calculator;

public class GetPrimeFactors : StatelessQueryHandler<ICalculatorManagerGrain, IEnumerable<double>>
{
    public override Task<IEnumerable<double>> Execute()
    {
        return QueryFactory.GetChild<CalculatorState>()
            .Ask<GetPrimeFactorsState,  IEnumerable<double>>()
            .Run(GetPrimaryKey().ToString());
    }
}