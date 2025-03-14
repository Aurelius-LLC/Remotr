namespace Remotr.Example.Calculator;

/// <summary>
/// Reducer that sums two double values
/// </summary>
public class SumReducer : IReduceInputs<double>
{
    public double Execute(double dto1, double dto2)
    {
        return dto1 + dto2;
    }
}