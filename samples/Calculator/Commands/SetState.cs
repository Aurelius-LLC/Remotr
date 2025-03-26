namespace Remotr.Samples.Calculator;

[UseShortcuts]
public class SetState : EntityCommandHandler<CalculatorState, double, double>
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