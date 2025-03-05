
namespace Remotr.Example.Calculator;
public class SetValueState2Type : StatefulCommandHandler<CalculatorState, double>
{
    public override async Task<double> Execute()
    {
        await UpdateState(new() { Value = 0 });
        return 0;
    }
}