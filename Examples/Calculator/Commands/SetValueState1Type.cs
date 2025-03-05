
namespace Remotr.Example.Calculator;
public class SetValueState1Type : StatefulCommandHandler<CalculatorState>
{
    public override async Task Execute()
    {
        await UpdateState(new() { Value = 0 });
    }
}