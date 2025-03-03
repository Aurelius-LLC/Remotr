namespace Remotr;

public interface IProduceUniversalBuilder<T, K> where T : IGrain
{
    internal UniversalBuilder<T, K> Builder { get; }
}
