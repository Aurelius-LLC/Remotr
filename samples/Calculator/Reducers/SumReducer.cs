namespace Remotr.Samples.Calculator;

/// <summary>
/// Reducer that sums two double values
/// </summary>
public class SumReducer : IReduceInputs<double>, IMergeInputs<double, double, double>
{
    public double Execute(double dto1, double dto2)
    {
        return dto1 + dto2;
    }
}