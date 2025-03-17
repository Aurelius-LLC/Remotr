namespace Remotr;

public static class BuilderExtensions
{
    public static K Run<T, K>(this IRunBuilder<T, K> target, string stringKey, string? grainClassNamePrefix = null)
        where T : IAggregateRoot, IGrainWithStringKey
        where K : IAsyncResult
    {
        return target.RunAggregate(target.GrainFactory.GetGrain<T>(stringKey, grainClassNamePrefix: grainClassNamePrefix));
    }

    public static K Run<T, K>(this IRunBuilder<T, K> target, Guid guidKey, string? grainClassNamePrefix = null)
        where T : IAggregateRoot, IGrainWithGuidKey
        where K : IAsyncResult
    {
        return target.RunAggregate(target.GrainFactory.GetGrain<T>(guidKey, grainClassNamePrefix: grainClassNamePrefix));
    }

    public static K Run<T, K>(this IRunBuilder<T, K> target, int intKey, string? grainClassNamePrefix = null)
        where T : IAggregateRoot, IGrainWithIntegerKey
        where K : IAsyncResult
    {
        return target.RunAggregate(target.GrainFactory.GetGrain<T>(intKey, grainClassNamePrefix: grainClassNamePrefix));
    }

    public static K Run<T, K>(this IRunBuilder<T, K> target, Guid guidKey, string stringKey, string? grainClassNamePrefix = null)
        where T : IAggregateRoot, IGrainWithGuidCompoundKey
        where K : IAsyncResult
    {
        return target.RunAggregate(target.GrainFactory.GetGrain<T>(guidKey, stringKey, grainClassNamePrefix: grainClassNamePrefix));
    }

    public static K Run<T, K>(this IRunBuilder<T, K> target, int intKey, string stringKey, string? grainClassNamePrefix = null)
        where T : IAggregateRoot, IGrainWithIntegerCompoundKey
        where K : IAsyncResult
    {
        return target.RunAggregate(target.GrainFactory.GetGrain<T>(intKey, stringKey, grainClassNamePrefix: grainClassNamePrefix));
    }


    public static Output Run<State, Output>(this IRunBuilder<IAggregateEntity<State>, Output> target, string key)
        where State : new()
        where Output : IAsyncResult
    {
        var entityGrain = target.ResolveEntityGrain(key);
        var interleave = RequestContext.Get("readOnly") is bool readOnly && readOnly;
        return target.RunEntityGrain<IAggregateEntity<State>, State>(entityGrain, interleave);
    }
}
