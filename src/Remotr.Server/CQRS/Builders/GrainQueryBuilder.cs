namespace Remotr;

public class GrainQueryBuilder<T, Q, K> : IGrainQueryBuilder<T, Q, K> where T : IGrain
{
    internal readonly IGrainFactory grainFactory;
    internal readonly Func<string, T> resolveEntityGrain;
    private readonly UniversalBuilder<T, K> _builder;
    internal readonly Func<IGrain, ValueTask>? _ranWith;


    internal GrainQueryBuilder(
        IGrainFactory grainFactory,
        Func<string, T> resolveEntityGrain,
        UniversalBuilder<T, K> builder,
        Func<IGrain, ValueTask>? ranWith = null
    )
    {
        this.grainFactory = grainFactory;
        this.resolveEntityGrain = resolveEntityGrain;
        _builder = builder;
        _ranWith = ranWith;
    }

    UniversalBuilder<T, K> IProduceUniversalBuilder<T, K>.Builder => _builder;
    IGrainFactory IHaveGrainFactory.GrainFactory => grainFactory;
    Func<string, T> IResolveEntityGrain<T>.ResolveEntityGrain => resolveEntityGrain;


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

    public IGrainQueryBuilder<T, Q, Y> ThenAsk<X, Y>() where X : IAsyncQueryHandler<T, K, Y>, Q
    {
        return new GrainQueryBuilder<T, Q, Y>(
            grainFactory,
            resolveEntityGrain,
            _builder.ThenAsk<X, Y>()
        );
    }

    public IGrainQueryBuilder<T, Q, Y> Map<X, Y>() where X : IMapInput<K, Y>
    {
        return new GrainQueryBuilder<T, Q, Y>(
            grainFactory,
            resolveEntityGrain,
            _builder.ThenMap<X, Y>()
        );
    }

    public IGrainQueryBuilder<T, Q, Merged> MergeSplit<Output1, Output2, Merged>(
        Func<IGrainQueryBuilder<T, Q, K>, IGrainQueryBuilder<T, Q, Output1>> firstOp,
        Func<IGrainQueryBuilder<T, Q, K>, IGrainQueryBuilder<T, Q, Output2>> secondOp,
        IMergeInputs<Output1, Output2, Merged> merger
    )
    {
        var needsPropStep = new NeedsPropStep<K>();

        var needsPropBuilder = new GrainQueryBuilder<T, Q, K>(
            grainFactory,
            resolveEntityGrain,
            new UniversalBuilder<T, K>(
                needsPropStep
            )
        );

        var builder1 = firstOp(needsPropBuilder);
        var builder2 = secondOp(needsPropBuilder);

        return new GrainQueryBuilder<T, Q, Merged>(
            grainFactory,
            resolveEntityGrain,
            _builder.MergeSplit
            (
                needsPropStep,
                builder1,
                builder2,
                merger
            )
        );
    }

    public IGrainQueryBuilder<T, Q, Merged> MergeSplit<Merger, Output1, Output2, Merged>(
        Func<IGrainQueryBuilder<T, Q, K>, IGrainQueryBuilder<T, Q, Output1>> firstOp,
        Func<IGrainQueryBuilder<T, Q, K>, IGrainQueryBuilder<T, Q, Output2>> secondOp
    ) where Merger : IMergeInputs<Output1, Output2, Merged>
    {
        var needsPropStep = new NeedsPropStep<K>();

        var needsPropBuilder = new GrainQueryBuilder<T, Q, K>(
            grainFactory,
            resolveEntityGrain,
            new UniversalBuilder<T, K>(
                needsPropStep
            )
        );

        var builder1 = firstOp(needsPropBuilder);
        var builder2 = secondOp(needsPropBuilder);

        return new GrainQueryBuilder<T, Q, Merged>(
            grainFactory,
            resolveEntityGrain,
            _builder.MergeSplit<
                Merger,
                Output1,
                Output2,
                Merged>
            (
                needsPropStep,
                builder1,
                builder2
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

    public IGrainQueryBaseBuilder<T, Q> ConvertToBaseBuilder() {
        return new GrainQueryBaseBuilder<T, Q> (
            grainFactory,
            resolveEntityGrain,
            new UniversalBuilder<T, object>(
                new MergeStepsWrapper<K, object, object> {
                    Step1 = _builder.ExecutionStep,
                    Step2 = new EmptyStep(),
                    Merger = new TakeSecond<K, object>()
                }
            )
        );
    }

    public Task<K> RunAggregate<X>(X aggregate) where X : T, IAggregateRoot
    {
        _ranWith?.Invoke(aggregate);
        return aggregate.ExecuteQuery(_builder.ExecutionStep, false);
    }

    public async Task<K> RunEntityGrain<X, TState>(X entityGrain, bool interleave)
        where X : T, IAggregateEntity<TState>
        where TState : new()
    {
        if (_ranWith != null)
        {
            await _ranWith(entityGrain);
        }
        if (interleave) {
            return await entityGrain.ExecuteInterleaving(_builder.ExecutionStep);
        }
        else {
            return await entityGrain.ExecuteNotInterleaving(_builder.ExecutionStep);
        }
    }
}
