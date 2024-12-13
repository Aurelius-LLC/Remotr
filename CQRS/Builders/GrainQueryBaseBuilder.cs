namespace Remotr;

public class GrainQueryBaseBuilder<T, Q> : IGrainQueryBaseBuilder<T, Q> where T : IGrain
{
    internal readonly IGrainFactory grainFactory;
    internal readonly Func<string, T> resolveChildGrain;
    private readonly UniversalBuilder<T, object> _builder;
    internal readonly Func<IGrain, ValueTask>? _ranWith;


    internal GrainQueryBaseBuilder(
        IGrainFactory grainFactory,
        Func<string, T> resolveChildGrain,
        UniversalBuilder<T, object> builder,
        Func<IGrain, ValueTask>? ranWith = null
    )
    {
        this.grainFactory = grainFactory;
        this.resolveChildGrain = resolveChildGrain;
        _builder = builder;
        _ranWith = ranWith;
    }

    UniversalBuilder<T, object> IProduceUniversalBuilder<T, object>.Builder => _builder;
    IGrainFactory IHaveGrainFactory.GrainFactory => grainFactory;
    Func<string, T> IResolveChildGrain<T>.ResolveChildGrain => resolveChildGrain;


    internal GrainQueryBaseBuilder<T, Q> GetEmptyBuilder()
    {
        return new GrainQueryBaseBuilder<T, Q>(
            grainFactory,
            resolveChildGrain,
            new UniversalBuilder<T, object>(
                new EmptyStep()
            )
        );
    }

    public IGrainQueryBuilder<T, Q, Y> Ask<X, Y>() where X : IAsyncQueryHandler<T, Y>, Q
    {
        return new GrainQueryBuilder<T, Q, Y>(
            grainFactory,
            resolveChildGrain,
            _builder.Ask<X, Y>()
        );
    }

    public IGrainQueryBuilder<T, Q, Z> Ask<X, Y, Z>(Y dto) where X : IAsyncQueryHandler<T, Y, Z>, Q
    {
        return new GrainQueryBuilder<T, Q, Z>(
            grainFactory,
            resolveChildGrain,
            _builder.Ask<X, Y, Z>(dto)
        );
    }

    public IGrainQueryBuilder<T, Q, Merged> MergeSplit<Merger, Output1, Output2, Merged>(
        Func<IGrainQueryBaseBuilder<T, Q>, IGrainQueryBuilder<T, Q, Output1>> firstOp,
        Func<IGrainQueryBaseBuilder<T, Q>, IGrainQueryBuilder<T, Q, Output2>> secondOp
    ) where Merger : IMergeInputs<Output1, Output2, Merged>
    {
        var emptyBuilder = GetEmptyBuilder();
        var builder1 = firstOp(emptyBuilder);
        var builder2 = secondOp(emptyBuilder);

        return new GrainQueryBuilder<T, Q, Merged>(
            grainFactory,
            resolveChildGrain,
            _builder.MergeSplit<
                Merger,
                Output1,
                Output2,
                Merged>
            (
                builder1,
                builder2
            )
        );
    }

    public IGrainQueryBuilder<T, Q, Merged> MergeSplit<Output1, Output2, Merged>(
        Func<IGrainQueryBaseBuilder<T, Q>, IGrainQueryBuilder<T, Q, Output1>> firstOp,
        Func<IGrainQueryBaseBuilder<T, Q>, IGrainQueryBuilder<T, Q, Output2>> secondOp,
        IMergeInputs<Output1, Output2, Merged> merger
    )
    {
        var emptyBuilder = GetEmptyBuilder();
        var builder1 = firstOp(emptyBuilder);
        var builder2 = secondOp(emptyBuilder);

        return new GrainQueryBuilder<T, Q, Merged>(
            grainFactory,
            resolveChildGrain,
            _builder.MergeSplit
            (
                builder1,
                builder2,
                merger
            )
        );
    }


    public IGrainQueryBuilder<T, Q, IEnumerable<Output>> ForEachSplit<Input, Output>(
        IEnumerable<Input> inputs,
        Func<IGrainQueryBuilder<T, Q, Input>, IGrainQueryBuilder<T, Q, Output>> operation
    )
    {
        var executionStepNeedsInput = new NeedsPropStep<Input>();

        var builder = operation(
            new GrainQueryBuilder<T, Q, Input>(
                grainFactory,
                resolveChildGrain,
                new UniversalBuilder<T, Input>(
                    executionStepNeedsInput
                )
            )
        );

        return new GrainQueryBuilder<T, Q, IEnumerable<Output>>
        (
            grainFactory,
            resolveChildGrain,
            _builder.ForEachSplit
            (
                inputs,
                executionStepNeedsInput,
                builder
            )
        );
    }

    public Task RunManagerGrain<X>(X managerGrain) where X : T, ITransactionManagerGrain
    {
        _ranWith?.Invoke(managerGrain);
        return managerGrain.ExecuteQuery(_builder.ExecutionStep, false);
    }

    public async Task RunChildGrain<X, TState>(X childGrain, bool interleave)
        where X : T, ITransactionChildGrain<TState>
        where TState : new()
    {
        if (_ranWith != null)
        {
            await _ranWith(childGrain);
        }
        await childGrain.Execute(_builder.ExecutionStep, interleave);
    }
}
