namespace Remotr;

public interface IResolveChildGrain<T> where T : IGrain
{
    internal Func<string, T> ResolveChildGrain { get; }
}
