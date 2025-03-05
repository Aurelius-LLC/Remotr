
namespace Remotr.Example.Calculator;

public class GetValueState3Type : StatefulQueryHandler<CalculatorState, int, double>
{
    public override async Task<double> Execute(int input)
    {
        return (await GetState()).Value;
    }
}