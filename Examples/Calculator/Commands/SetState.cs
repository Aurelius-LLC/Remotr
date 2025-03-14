namespace Remotr.Example.Calculator;

[RemotrGen]
public class SetState : StatefulCommandHandler<CalculatorState, double, double>
{
    public override async Task<double> Execute(double input)
    {
        await UpdateState(
            new() {
                Value = input
            }
        );
        return input;
    }
}