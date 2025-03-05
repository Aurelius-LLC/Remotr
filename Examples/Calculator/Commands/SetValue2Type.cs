

namespace Remotr.Example.Calculator;

[RemotrGen]
public class SetValue2Type : StatelessCommandHandler<ICalculatorManagerGrain, double>
{
    public override Task<double> Execute()
    {
        throw new NotImplementedException();
    }
}