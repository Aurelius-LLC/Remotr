
namespace Remotr.Example.Calculator;

public class GetValue : StatefulQueryHandler<CalculatorState, double>
{
    public override async Task<double> Execute()
    {
        return (await GetState()).Value;
    }
}