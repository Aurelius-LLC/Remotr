namespace Remotr;

public interface IGrainQueryBuilder<T, Q, K> : IProduceUniversalBuilder<T, K>, IRunBuilder<T, Task<K>>
    where T : IGrain
{
    public IGrainQueryBuilder<T, Q, Y> Ask<X, Y>() where X : IAsyncQueryHandler<T, Y>, Q;

    public IGrainQueryBuilder<T, Q, Z> Ask<X, Y, Z>(Y dto) where X : IAsyncQueryHandler<T, Y, Z>, Q;

    public IGrainQueryBuilder<T, Q, Y> ThenAsk<X, Y>() where X : IAsyncQueryHandler<T, K, Y>, Q;

    public IGrainQueryBuilder<T, Q, Y> Map<X, Y>() where X : IMapInput<K, Y>;

    public IGrainQueryBuilder<T, Q, Merged> MergeSplit<Output1, Output2, Merged>
    (
        Func<IGrainQueryBuilder<T, Q, K>, IGrainQueryBuilder<T, Q, Output1>> firstOp,
        Func<IGrainQueryBuilder<T, Q, K>, IGrainQueryBuilder<T, Q, Output2>> secondOp,
        IMergeInputs<Output1, Output2, Merged> merger
    );

    public IGrainQueryBuilder<T, Q, Merged> MergeSplit<Merger, Output1, Output2, Merged>
    (
        Func<IGrainQueryBuilder<T, Q, K>, IGrainQueryBuilder<T, Q, Output1>> firstOp,
        Func<IGrainQueryBuilder<T, Q, K>, IGrainQueryBuilder<T, Q, Output2>> secondOp
    ) where Merger : IMergeInputs<Output1, Output2, Merged>;

    public IGrainQueryBuilder<T, Q, IEnumerable<Output>> ForEach<Input, Output>
    (
        IEnumerable<Input> inputs,
        Func<IGrainQueryBuilder<T, Q, Input>, IGrainQueryBuilder<T, Q, Output>> operation
    );
}

public static class GrainQueryBuilderExtensions
{

    public static IGrainQueryBuilder<T, Q, IEnumerable<Output>> ThenForEachSplit<T, Q, Input, Output>
    (
        this IGrainQueryBuilder<T, Q, IEnumerable<Input>> target,
        Func<IGrainQueryBuilder<T, Q, Input>, IGrainQueryBuilder<T, Q, Output>> operation
    )
        where T : IGrain
    {
        var executionStepNeedsInput = new NeedsPropStep<Input>();

        var builder = operation(
            new GrainQueryBuilder<T, Q, Input>(
                target.GrainFactory,
                target.ResolveChildGrain,
                new UniversalBuilder<T, Input>(
                    executionStepNeedsInput
                )
            )
        );

        return new GrainQueryBuilder<T, Q, IEnumerable<Output>>
        (
            target.GrainFactory,
            target.ResolveChildGrain,
            target.Builder.ThenForEachSplit
            (
                executionStepNeedsInput,
                builder
            )
        );
    }

    public static IGrainQueryBuilder<T, Q, IEnumerable<Output>> ThenFilter<T, Q, Mapper, Output>(this IGrainQueryBuilder<T, Q, IEnumerable<Output>> target)
        where T : IGrain
        where Mapper : IMapInput<Output, bool>
    {
        return new GrainQueryBuilder<T, Q, IEnumerable<Output>>(
            target.GrainFactory,
            target.ResolveChildGrain,
            target.Builder.ThenFilter<T, Mapper, Output>()
        );
    }

    public static IGrainQueryBuilder<T, Q, IEnumerable<Output>> ThenFilter<T, Q, Output>(
        this IGrainQueryBuilder<T, Q, IEnumerable<Output>> target,
        IMapInput<Output, bool> mapper
    ) where T : IGrain
    {
        return new GrainQueryBuilder<T, Q, IEnumerable<Output>>(
            target.GrainFactory,
            target.ResolveChildGrain,
            target.Builder.ThenFilter(mapper)
        );
    }


    public static IGrainQueryBuilder<T, Q, IEnumerable<Output>> ThenMap<T, Q, Mapper, Input, Output>(
        this IGrainQueryBuilder<T, Q, IEnumerable<Input>> target
    )
        where T : IGrain
        where Mapper : IMapInput<Input, Output>
    {
        return new GrainQueryBuilder<T, Q, IEnumerable<Output>>(
            target.GrainFactory,
            target.ResolveChildGrain,
            target.Builder.ThenMap<T, Mapper, Input, Output>()
        );
    }

    public static IGrainQueryBuilder<T, Q, IEnumerable<Output>> ThenMap<T, Q, Input, Output>(
        this IGrainQueryBuilder<T, Q, IEnumerable<Input>> target,
        IMapInput<Input, Output> mapper
    ) where T : IGrain
    {
        return new GrainQueryBuilder<T, Q, IEnumerable<Output>>(
            target.GrainFactory,
            target.ResolveChildGrain,
            target.Builder.ThenMap(mapper)
        );
    }

    public static IGrainQueryBuilder<T, Q, Output> ThenReduce<T, Q, Reducer, Output>(
        this IGrainQueryBuilder<T, Q, IEnumerable<Output>> target
    )
        where T : IGrain
        where Reducer : IReduceInputs<Output>
    {
        return new GrainQueryBuilder<T, Q, Output>(
            target.GrainFactory,
            target.ResolveChildGrain,
            target.Builder.ThenReduce<T, Reducer, Output>()
        );
    }

    public static IGrainQueryBuilder<T, Q, Output> ThenReduce<T, Q, Output>(
        this IGrainQueryBuilder<T, Q, IEnumerable<Output>> target,
        IReduceInputs<Output> reducer
    )
        where T : IGrain
    {
        return new GrainQueryBuilder<T, Q, Output>(
            target.GrainFactory,
            target.ResolveChildGrain,
            target.Builder.ThenReduce(reducer)
        );
    }
}
