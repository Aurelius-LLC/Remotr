
namespace Remotr.Example.Calculator;
public class SetValueState : StatefulCommandHandler<CalculatorState, double, double>
{
    public override async Task<double> Execute(double input)
    {
        await UpdateState(new() { Value = input });
        return input;
    }
}