namespace Remotr;

public interface IGrainCommandBuilder<T, C, Q, K> : IProduceUniversalBuilder<T, K>, IRunBuilder<T, Task<K>> where T : IGrain
{
    public IGrainCommandBuilder<T, C, Q, Y> Ask<X, Y>() where X : IAsyncQueryHandler<T, Y>, Q;

    public IGrainCommandBuilder<T, C, Q, Z> Ask<X, Y, Z>(Y dto) where X : IAsyncQueryHandler<T, Y, Z>, Q;

    public IGrainCommandBuilder<T, C, Q, Y> ThenAsk<X, Y>() where X : IAsyncQueryHandler<T, K, Y>, Q;

    public IGrainCommandBuilder<T, C, Q, Y> Map<X, Y>() where X : IMapInput<K, Y>;

    public IGrainCommandBaseBuilder<T, C, Q> Tell<X>() where X : IAsyncCommandHandler<T>, C;

    public IGrainCommandBuilder<T, C, Q, Y> Tell<X, Y>() where X : IAsyncCommandHandler<T, Y>, C;

    public IGrainCommandBuilder<T, C, Q, Z> Tell<X, Y, Z>(Y dto) where X : IAsyncCommandHandler<T, Y, Z>, C;

    public IGrainCommandBuilder<T, C, Q, Y> ThenTell<X, Y>() where X : IAsyncCommandHandler<T, K, Y>, C;

    public IGrainCommandBuilder<T, C, Q, Merged> MergeSplit<Output1, Output2, Merged>
    (
        Func<IGrainCommandBuilder<T, C, Q, K>, IGrainCommandBuilder<T, C, Q, Output1>> firstOp,
        Func<IGrainCommandBuilder<T, C, Q, K>, IGrainCommandBuilder<T, C, Q, Output2>> secondOp,
        IMergeInputs<Output1, Output2, Merged> merger
    );

    public IGrainCommandBuilder<T, C, Q, Merged> MergeSplit<Merger, Output1, Output2, Merged>
    (
        Func<IGrainCommandBuilder<T, C, Q, K>, IGrainCommandBuilder<T, C, Q, Output1>> firstOp,
        Func<IGrainCommandBuilder<T, C, Q, K>, IGrainCommandBuilder<T, C, Q, Output2>> secondOp
    ) where Merger : IMergeInputs<Output1, Output2, Merged>;

    public IGrainCommandBuilder<T, C, Q, IEnumerable<Output>> ForEach<Input, Output>
    (
        IEnumerable<Input> inputs,
        Func<IGrainCommandBuilder<T, C, Q, Input>, IGrainCommandBuilder<T, C, Q, Output>> operation
    );
}

public static class GrainCommandBuilderExtensions
{

    public static IGrainCommandBuilder<T, C, Q, IEnumerable<Output>> ThenForEach<T, C, Q, Input, Output>
    (
        this IGrainCommandBuilder<T, C, Q, IEnumerable<Input>> target,
        Func<IGrainCommandBuilder<T, C, Q, Input>, IGrainCommandBuilder<T, C, Q, Output>> operation
    )
        where T : IGrain
    {
        var executionStepNeedsInput = new NeedsPropStep<Input>();

        var builder = operation(
            new GrainCommandBuilder<T, C, Q, Input>(
                target.GrainFactory,
                target.ResolveEntityGrain,
                new UniversalBuilder<T, Input>(
                    executionStepNeedsInput
                )
            )
        );

        return new GrainCommandBuilder<T, C, Q, IEnumerable<Output>>
        (
            target.GrainFactory,
            target.ResolveEntityGrain,
            target.Builder.ThenForEachSplit
            (
                executionStepNeedsInput,
                builder
            )
        );
    }

    public static IGrainCommandBuilder<T, C, Q, IEnumerable<Output>> ThenFilter<T, C, Q, Mapper, Output>(this IGrainCommandBuilder<T, C, Q, IEnumerable<Output>> target)
        where T : IGrain
        where Mapper : IMapInput<Output, bool>, new()
    {
        return new GrainCommandBuilder<T, C, Q, IEnumerable<Output>>(
            target.GrainFactory,
            target.ResolveEntityGrain,
            target.Builder.ThenFilter<T, Mapper, Output>()
        );
    }

    public static IGrainCommandBuilder<T, C, Q, IEnumerable<Output>> ThenFilter<T, C, Q, Output>(
        this IGrainCommandBuilder<T, C, Q, IEnumerable<Output>> target,
        IMapInput<Output, bool> mapper
    ) where T : IGrain
    {
        return new GrainCommandBuilder<T, C, Q, IEnumerable<Output>>(
            target.GrainFactory,
            target.ResolveEntityGrain,
            target.Builder.ThenFilter(mapper)
        );
    }


    public static IGrainCommandBuilder<T, C, Q, IEnumerable<Output>> ThenMap<T, C, Q, Mapper, Input, Output>(
        this IGrainCommandBuilder<T, C, Q, IEnumerable<Input>> target
    )
        where T : IGrain
        where Mapper : IMapInput<Input, Output>
    {
        return new GrainCommandBuilder<T, C, Q, IEnumerable<Output>>(
            target.GrainFactory,
            target.ResolveEntityGrain,
            target.Builder.ThenMap<T, Mapper, Input, Output>()
        );
    }

    public static IGrainCommandBuilder<T, C, Q, IEnumerable<Output>> ThenMap<T, C, Q, Input, Output>(
        this IGrainCommandBuilder<T, C, Q, IEnumerable<Input>> target,
        IMapInput<Input, Output> mapper
    ) where T : IGrain
    {
        return new GrainCommandBuilder<T, C, Q, IEnumerable<Output>>(
            target.GrainFactory,
            target.ResolveEntityGrain,
            target.Builder.ThenMap(mapper)
        );
    }

    public static IGrainCommandBuilder<T, C, Q, Output> ThenReduce<T, C, Q, Reducer, Output>(
        this IGrainCommandBuilder<T, C, Q, IEnumerable<Output>> target
    )
        where T : IGrain
        where Reducer : IReduceInputs<Output>
    {
        return new GrainCommandBuilder<T, C, Q, Output>(
            target.GrainFactory,
            target.ResolveEntityGrain,
            target.Builder.ThenReduce<T, Reducer, Output>()
        );
    }

    public static IGrainCommandBuilder<T, C, Q, Output> ThenReduce<T, C, Q, Output>(
        this IGrainCommandBuilder<T, C, Q, IEnumerable<Output>> target,
        IReduceInputs<Output> reducer
    )
        where T : IGrain
    {
        return new GrainCommandBuilder<T, C, Q, Output>(
            target.GrainFactory,
            target.ResolveEntityGrain,
            target.Builder.ThenReduce(reducer)
        );
    }
}
