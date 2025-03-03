
namespace Remotr.Example.Calculator;
public class SetValue1Type : StatelessCommandHandler<ICalculatorManagerGrain>
{
    public override Task Execute()
    {
        return Task.CompletedTask;
    }
}