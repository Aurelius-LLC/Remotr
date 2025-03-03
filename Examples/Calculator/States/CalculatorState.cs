

namespace Remotr.Example.Calculator;

[GenerateSerializer]
public record CalculatorState
{
    [Id(0)]
    public double Value { get; set; }
}