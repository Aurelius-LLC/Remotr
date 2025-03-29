namespace Remotr;

public class GrainCommandBuilder<T, C, Q, K> : IGrainCommandBuilder<T, C, Q, K> where T : IGrain
{
    internal readonly IGrainFactory grainFactory;
    internal readonly Func<string, T> resolveEntityGrain;
    internal readonly UniversalBuilder<T, K> _builder;
    internal readonly Func<IGrain, ValueTask>? _ranWith;


    internal GrainCommandBuilder(
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

    internal GrainCommandBuilder<T, C, Q, K> GetProxyBuilder(ExecutionStep<K> step)
    {
        return new GrainCommandBuilder<T, C, Q, K>(
            grainFactory,
            resolveEntityGrain,
            new UniversalBuilder<T, K>(
                step
            )
        );
    }

    public IGrainCommandBuilder<T, C, Q, Y> Ask<X, Y>() where X : IAsyncQueryHandler<T, Y>, Q
    {
        return new GrainCommandBuilder<T, C, Q, Y>(
            grainFactory,
            resolveEntityGrain,
            _builder.Ask<X, Y>()
        );
    }

    public IGrainCommandBuilder<T, C, Q, Z> Ask<X, Y, Z>(Y dto) where X : IAsyncQueryHandler<T, Y, Z>, Q
    {
        return new GrainCommandBuilder<T, C, Q, Z>(
            grainFactory,
            resolveEntityGrain,
            _builder.Ask<X, Y, Z>(dto)
        );
    }

    public IGrainCommandBuilder<T, C,Q, Y> ThenAsk<X, Y>() where X : IAsyncQueryHandler<T, K, Y>, Q
    {
        return new GrainCommandBuilder<T, C, Q, Y>(
            grainFactory,
            resolveEntityGrain,
            _builder.ThenAsk<X, Y>()
        );
    }

    public IGrainCommandBaseBuilder<T, C, Q> Tell<X>() where X : IAsyncCommandHandler<T>, C
    {
        return new GrainCommandBaseBuilder<T, C, Q>(
            grainFactory,
            resolveEntityGrain,
            _builder.Tell<X>()
        );
    }

    public IGrainCommandBuilder<T, C, Q, Y> Tell<X, Y>() where X : IAsyncCommandHandler<T, Y>, C
    {
        return new GrainCommandBuilder<T, C, Q, Y>(
            grainFactory,
            resolveEntityGrain,
            _builder.Tell<X, Y>()
        );
    }

    public IGrainCommandBuilder<T, C, Q, Z> Tell<X, Y, Z>(Y dto) where X : IAsyncCommandHandler<T, Y, Z>, C
    {
        return new GrainCommandBuilder<T, C, Q, Z>(
            grainFactory,
            resolveEntityGrain,
            _builder.Tell<X, Y, Z>(dto)
        );
    }

    public IGrainCommandBuilder<T, C, Q, Y> ThenTell<X, Y>() where X : IAsyncCommandHandler<T, K, Y>, C
    {
        return new GrainCommandBuilder<T, C, Q, Y>(
            grainFactory,
            resolveEntityGrain,
            _builder.ThenTell<X, Y>()
        );
    }

    public IGrainCommandBuilder<T, C, Q, Merged> MergeSplit<Merger, Output1, Output2, Merged>(
        Func<IGrainCommandBuilder<T, C, Q, K>, IGrainCommandBuilder<T, C, Q, Output1>> firstOp,
        Func<IGrainCommandBuilder<T, C, Q, K>, IGrainCommandBuilder<T, C, Q, Output2>> secondOp
    ) where Merger : IMergeInputs<Output1, Output2, Merged>
    {
        var executionStepNeedsInput = new NeedsPropStep<K>();
        var builder1 = firstOp(GetProxyBuilder(executionStepNeedsInput));
        var builder2 = secondOp(GetProxyBuilder(executionStepNeedsInput));
        return new GrainCommandBuilder<T, C, Q, Merged>(
            grainFactory,
            resolveEntityGrain,
            _builder.MergeSplit<Merger, Output1, Output2, Merged>(
                executionStepNeedsInput,
                builder1,
                builder2
            )
        );
    }

    public IGrainCommandBuilder<T, C, Q, Merged> MergeSplit<Output1, Output2, Merged>(
        Func<IGrainCommandBuilder<T, C, Q, K>, IGrainCommandBuilder<T, C, Q, Output1>> firstOp,
        Func<IGrainCommandBuilder<T, C, Q, K>, IGrainCommandBuilder<T, C, Q, Output2>> secondOp,
        IMergeInputs<Output1, Output2, Merged> merger
    )
    {
        var executionStepNeedsInput = new NeedsPropStep<K>();
        var builder1 = firstOp(GetProxyBuilder(executionStepNeedsInput));
        var builder2 = secondOp(GetProxyBuilder(executionStepNeedsInput));
        return new GrainCommandBuilder<T, C, Q, Merged>(
            grainFactory,
            resolveEntityGrain,
            _builder.MergeSplit(
                executionStepNeedsInput,
                builder1,
                builder2,
                merger
            )
        );
    }


    public IGrainCommandBuilder<T, C, Q, Y> Map<X, Y>() where X : IMapInput<K, Y>
    {
        return new GrainCommandBuilder<T, C, Q, Y>(
            grainFactory,
            resolveEntityGrain,
            _builder.ThenMap<X, Y>()
        );
    }

    public IGrainCommandBuilder<T, C, Q, IEnumerable<Output>> ForEachTell<Handler, Input, Output>(IEnumerable<Input> inputs) where Handler : IAsyncCommandHandler<T, Input, Output>, C
    {
        return new GrainCommandBuilder<T, C, Q, IEnumerable<Output>>(
            grainFactory,
            resolveEntityGrain,
            _builder.ForEach<Handler, Input, Output>(inputs)
        );
    }

    public IGrainCommandBuilder<T, C, Q, IEnumerable<Output>> ForEach<Input, Output>(IEnumerable<Input> inputs, Func<IGrainCommandBuilder<T, C, Q, Input>, IGrainCommandBuilder<T, C, Q, Output>> operation)
    {
        var executionStepNeedsInput = new NeedsPropStep<Input>();
        var builder = operation(
            new GrainCommandBuilder<T, C, Q, Input>(
                grainFactory,
                resolveEntityGrain,
                new UniversalBuilder<T, Input>(
                    executionStepNeedsInput
                )
            )
        );

        return new GrainCommandBuilder<T, C, Q, IEnumerable<Output>>(
            grainFactory,
            resolveEntityGrain,
            _builder.ForEachSplit(
                inputs,
                executionStepNeedsInput,
                builder
            )
        );
    }

    public IGrainCommandBaseBuilder<T, C, Q> ConvertToBaseBuilder() {
        return new GrainCommandBaseBuilder<T, C, Q> (
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
        return aggregate.ExecuteCommand(_builder.ExecutionStep);
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
