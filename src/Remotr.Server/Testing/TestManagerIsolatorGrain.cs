namespace Remotr.Testing;

internal interface ITestManagerIsolatorGrain : IGrain, IGrainWithStringKey
{
    Task EnsureTestIsolation(Guid testId);
}

public sealed class TestManagerIsolatorGrain : Grain, ITestManagerIsolatorGrain
{
    private Guid? claimedTestId = null;

    public Task EnsureTestIsolation(Guid testId)
    {
        if (claimedTestId == null)
        {
            claimedTestId = testId;
        }
        else if (testId != claimedTestId)
        {
            throw new InvalidOperationException("At least 2 tests attempted to make use of the same manager grain. This will cause test cache overlaps and unpredictable results. Ensure all tests call different manager grains.");
        }
        return Task.CompletedTask;
    }
}
