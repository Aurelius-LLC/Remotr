namespace Remotr;

public interface IResolveEntityGrain<T> where T : IGrain
{
    internal Func<string, T> ResolveEntityGrain { get; }
}
