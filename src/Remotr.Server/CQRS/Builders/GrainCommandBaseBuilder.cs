namespace Remotr;

public class GrainCommandBaseBuilder<T, C, Q> : IGrainCommandBaseBuilder<T, C, Q> where T : IGrain
{
    internal readonly IGrainFactory grainFactory;
    internal readonly Func<string, T> _resolveEntityGrain;
    internal readonly UniversalBuilder<T, object> _builder;
    internal readonly Func<IGrain, ValueTask>? _ranWith;

    internal GrainCommandBaseBuilder(
        IGrainFactory grainFactory,
        Func<string, T> resolveEntityGrain,
        UniversalBuilder<T, object> builder,
        Func<IGrain, ValueTask>? ranWith = null
    )
    {
        this.grainFactory = grainFactory;
        this._resolveEntityGrain = resolveEntityGrain;
        _builder = builder;
        _ranWith = ranWith;
    }

    UniversalBuilder<T, object> IProduceUniversalBuilder<T, object>.Builder => _builder;
    IGrainFactory IHaveGrainFactory.GrainFactory => grainFactory;
    Func<string, T> IResolveEntityGrain<T>.ResolveEntityGrain => _resolveEntityGrain;

    private GrainCommandBaseBuilder<T, C, Q> GetEmptyBuilder()
    {
        return new GrainCommandBaseBuilder<T, C, Q>(
            grainFactory,
            _resolveEntityGrain,
            new UniversalBuilder<T, object>(
                new EmptyStep()
            )
        );
    }

    public IGrainCommandBuilder<T, C, Q, Y> Ask<X, Y>() where X : IAsyncQueryHandler<T, Y>, Q
    {
        return new GrainCommandBuilder<T, C, Q, Y>(
            grainFactory,
            _resolveEntityGrain,
            _builder.Ask<X, Y>()
        );
    }

    public IGrainCommandBuilder<T, C, Q, Z> Ask<X, Y, Z>(Y dto) where X : IAsyncQueryHandler<T, Y, Z>, Q
    {
        return new GrainCommandBuilder<T, C, Q, Z>(
            grainFactory,
            _resolveEntityGrain,
            _builder.Ask<X, Y, Z>(dto)
        );
    }

    public IGrainCommandBaseBuilder<T, C, Q> Tell<X>() where X : IAsyncCommandHandler<T>, C
    {
        return new GrainCommandBaseBuilder<T, C, Q>(
            grainFactory,
            _resolveEntityGrain,
            _builder.Tell<X>()
        );
    }

    public IGrainCommandBuilder<T, C, Q, Y> Tell<X, Y>() where X : IAsyncCommandHandler<T, Y>, C
    {
        return new GrainCommandBuilder<T, C, Q, Y>(
            grainFactory,
            _resolveEntityGrain,
            _builder.Tell<X, Y>()
        );
    }

    public IGrainCommandBuilder<T, C, Q, Z> Tell<X, Y, Z>(Y dto) where X : IAsyncCommandHandler<T, Y, Z>, C
    {
        return new GrainCommandBuilder<T, C, Q, Z>(
            grainFactory,
            _resolveEntityGrain,
            _builder.Tell<X, Y, Z>(dto)
        );
    }

    public IGrainCommandBuilder<T, C, Q, Merged> MergeSplit<Merger, Output1, Output2, Merged>(
        Func<IGrainCommandBaseBuilder<T, C, Q>, IGrainCommandBuilder<T, C, Q, Output1>> firstOp,
        Func<IGrainCommandBaseBuilder<T, C, Q>, IGrainCommandBuilder<T, C, Q, Output2>> secondOp
    ) where Merger : IMergeInputs<Output1, Output2, Merged>
    {
        var emptyBuilder = GetEmptyBuilder();
        var builder1 = firstOp(emptyBuilder);
        var builder2 = secondOp(emptyBuilder);

        return new GrainCommandBuilder<T, C, Q, Merged>(
            grainFactory,
            _resolveEntityGrain,
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

    public IGrainCommandBuilder<T, C, Q, Merged> MergeSplit<Output1, Output2, Merged>(
        Func<IGrainCommandBaseBuilder<T, C, Q>, IGrainCommandBuilder<T, C, Q, Output1>> firstOp,
        Func<IGrainCommandBaseBuilder<T, C, Q>, IGrainCommandBuilder<T, C, Q, Output2>> secondOp,
        IMergeInputs<Output1, Output2, Merged> merger
    )
    {
        var emptyBuilder = GetEmptyBuilder();
        var builder1 = firstOp(emptyBuilder);
        var builder2 = secondOp(emptyBuilder);

        return new GrainCommandBuilder<T, C, Q, Merged>(
            grainFactory,
            _resolveEntityGrain,
            _builder.MergeSplit
            (
                builder1,
                builder2,
                merger
            )
        );
    }

    public IGrainCommandBuilder<T, C, Q, IEnumerable<Output>> TellForEach<Handler, Input, Output>(IEnumerable<Input> inputs)
        where Handler : IAsyncCommandHandler<T, Input, Output>, C
    {
        return new GrainCommandBuilder<T, C, Q, IEnumerable<Output>>(
            grainFactory,
            _resolveEntityGrain,
            _builder.ForEach<Handler, Input, Output>(inputs)
        );
    }


    public IGrainCommandBuilder<T, C, Q, IEnumerable<Output>> ForEach<Input, Output>(
        IEnumerable<Input> inputs,
        Func<IGrainCommandBuilder<T, C, Q, Input>, IGrainCommandBuilder<T, C, Q, Output>> operation
    )
    {
        var executionStepNeedsInput = new NeedsPropStep<Input>();
        var builder = operation(
            new GrainCommandBuilder<T, C, Q, Input>(
                grainFactory,
                _resolveEntityGrain,
                new UniversalBuilder<T, Input>(
                    executionStepNeedsInput
                )
            )
        );

        return new GrainCommandBuilder<T, C, Q, IEnumerable<Output>>(
            grainFactory,
            _resolveEntityGrain,
            _builder.ForEachSplit(
                inputs,
                executionStepNeedsInput,
                builder
            )
        );
    }

    public Task RunAggregate<X>(X aggregate) where X : T, IAggregateRoot
    {
        _ranWith?.Invoke(aggregate);
        return aggregate.ExecuteCommand(_builder.ExecutionStep);
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
