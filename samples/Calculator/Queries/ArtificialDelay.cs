namespace Remotr.Samples.Calculator;

[UseShortcuts]
public class ArtificialDelay : EntityQueryHandler<CalculatorState, double>
{
    public override async Task<double> Execute()
    {
        await Task.Delay(1000);
        return (await GetState()).Value;
    }
}