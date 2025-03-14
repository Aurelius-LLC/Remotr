

namespace Remotr.Example.Calculator;

[GenerateSerializer]
public record CalculatorState : IContainValue
{
    [Id(0)]
    public double Value { get; set; }

    public double GetValue() => Value;
}

public interface IContainValue  {
    public double GetValue();
}