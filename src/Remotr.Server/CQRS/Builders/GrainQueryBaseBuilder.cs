namespace Remotr;

public class GrainQueryBaseBuilder<T, Q> : IGrainQueryBaseBuilder<T, Q> where T : IGrain
{
    internal readonly IGrainFactory grainFactory;
    internal readonly Func<string, T> resolveEntityGrain;
    private readonly UniversalBuilder<T, object> _builder;
    internal readonly Func<IGrain, ValueTask>? _ranWith;


    internal GrainQueryBaseBuilder(
        IGrainFactory grainFactory,
        Func<string, T> resolveEntityGrain,
        UniversalBuilder<T, object> builder,
        Func<IGrain, ValueTask>? ranWith = null
    )
    {
        this.grainFactory = grainFactory;
        this.resolveEntityGrain = resolveEntityGrain;
        _builder = builder;
        _ranWith = ranWith;
    }

    UniversalBuilder<T, object> IProduceUniversalBuilder<T, object>.Builder => _builder;
    IGrainFactory IHaveGrainFactory.GrainFactory => grainFactory;
    Func<string, T> IResolveEntityGrain<T>.ResolveEntityGrain => resolveEntityGrain;


    internal GrainQueryBaseBuilder<T, Q> GetEmptyBuilder()
    {
        return new GrainQueryBaseBuilder<T, Q>(
            grainFactory,
            resolveEntityGrain,
            new UniversalBuilder<T, object>(
                new EmptyStep()
            )
        );
    }

    public IGrainQueryBuilder<T, Q, Y> Ask<X, Y>() where X : IAsyncQueryHandler<T, Y>, Q
    {
        return new GrainQueryBuilder<T, Q, Y>(
            grainFactory,
            resolveEntityGrain,
            _builder.Ask<X, Y>()
        );
    }

    public IGrainQueryBuilder<T, Q, Z> Ask<X, Y, Z>(Y dto) where X : IAsyncQueryHandler<T, Y, Z>, Q
    {
        return new GrainQueryBuilder<T, Q, Z>(
            grainFactory,
            resolveEntityGrain,
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
            resolveEntityGrain,
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
            resolveEntityGrain,
            _builder.MergeSplit
            (
                builder1,
                builder2,
                merger
            )
        );
    }


    public IGrainQueryBuilder<T, Q, IEnumerable<Output>> ForEach<Input, Output>(
        IEnumerable<Input> inputs,
        Func<IGrainQueryBuilder<T, Q, Input>, IGrainQueryBuilder<T, Q, Output>> operation
    )
    {
        var executionStepNeedsInput = new NeedsPropStep<Input>();

        var builder = operation(
            new GrainQueryBuilder<T, Q, Input>(
                grainFactory,
                resolveEntityGrain,
                new UniversalBuilder<T, Input>(
                    executionStepNeedsInput
                )
            )
        );

        return new GrainQueryBuilder<T, Q, IEnumerable<Output>>
        (
            grainFactory,
            resolveEntityGrain,
            _builder.ForEachSplit
            (
                inputs,
                executionStepNeedsInput,
                builder
            )
        );
    }

    public Task RunAggregate<X>(X aggregate) where X : T, IAggregateRoot
    {
        _ranWith?.Invoke(aggregate);
        return aggregate.ExecuteQuery(_builder.ExecutionStep, false);
    }

    public async Task RunEntityGrain<X, TState>(X entityGrain, bool interleave)
        where X : T, IAggregateEntity<TState>
        where TState : new()
    {
        if (_ranWith != null)
        {
            await _ranWith(entityGrain);
        }
        if (interleave) {
            await entityGrain.ExecuteInterleaving(_builder.ExecutionStep);
        }
        else {
            await entityGrain.ExecuteNotInterleaving(_builder.ExecutionStep);
        }
    }
}
