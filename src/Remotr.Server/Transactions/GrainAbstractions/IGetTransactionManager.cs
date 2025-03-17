namespace Remotr;

public interface IGetAggregateRoot
{
    /// <summary>
    /// Returns the grain that manages all transactions involving this grain.
    /// </summary>
    /// <returns></returns>
    IAggregateRoot GetAggregate();
}
