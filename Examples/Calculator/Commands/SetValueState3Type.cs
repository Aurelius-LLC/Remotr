
namespace Remotr.Example.Calculator;
public class SetValueState3Type : StatefulCommandHandler<CalculatorState, int, double>
{
    public override async Task<double> Execute(int input)
    {
        await UpdateState(new() { Value = input });
        return input;
    }
}