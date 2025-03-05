
namespace Remotr.Example.Calculator;

public class GetValueState2Type : StatefulQueryHandler<CalculatorState, double>
{
    public override async Task<double> Execute()
    {
        return (await GetState()).Value;
    }
}