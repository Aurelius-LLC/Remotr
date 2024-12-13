namespace Remotr;

public interface IGetTransactionManager
{
    /// <summary>
    /// Returns the grain that manages all transactions involving this grain.
    /// </summary>
    /// <returns></returns>
    ITransactionManagerGrain GetManagerGrain();
}
