
namespace Remotr.Example.Calculator;

public class GetValueState1Type : StatefulQueryHandler<CalculatorState, double>
{
    public override async Task<double> Execute()
    {
        return (await GetState()).Value;
    }
}