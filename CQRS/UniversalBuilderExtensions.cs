namespace Remotr;

internal static class UniversalBuilderExtensions
{
    internal static UniversalBuilder<T, IEnumerable<Output>> ThenForEachTell<T, Command, Input, Output>(this UniversalBuilder<T, IEnumerable<Input>> target)
        where T : IGrain
        where Command : IAsyncCommandHandler<T, Input, Output>
    {
        return new UniversalBuilder<T, IEnumerable<Output>>(
            new TransformStepAsyncWrapper<IEnumerable<Input>, IEnumerable<Output>>
            {
                PreviousStep = target.ExecutionStep,
                CurrentStep = new LoopStepAsyncWrapper<Input, Output>
                {
                    CurrentStep = new AsyncCommandWrapper<T, Command, Input, Output>(),
                }
            }
        );
    }

    internal static UniversalBuilder<T, IEnumerable<Output>> ThenForEachSplit<T, Input, Output>(
        this UniversalBuilder<T, IEnumerable<Input>> target,
        NeedsPropStep<Input> stepNeedsProp,
        IProduceUniversalBuilder<T, Output> outputer
    ) where T : IGrain
    {
        var executionStep = outputer.Builder.ExecutionStep;
        return new UniversalBuilder<T, IEnumerable<Output>>(
            new TransformStepAsyncWrapper<IEnumerable<Input>, IEnumerable<Output>>
            {
                PreviousStep = target.ExecutionStep,
                CurrentStep = new LoopStepAsyncWrapper<Input, Output>
                {
                    CurrentStep = new PropBridgeStepWrapper<Input, Output>
                    {
                        BridgeStep = stepNeedsProp,
                        CurrentStep = executionStep
                    }
                }
            }
        );
    }

    internal static UniversalBuilder<T, IEnumerable<Output>> ThenMap<T, Mapper, Input, Output>(
        this UniversalBuilder<T, IEnumerable<Input>> target
    )
        where T : IGrain
        where Mapper : IMapInput<Input, Output>
    {
        return new UniversalBuilder<T, IEnumerable<Output>>(
            new TransformStepWrapper<IEnumerable<Input>, IEnumerable<Output>>
            {
                PreviousStep = target.ExecutionStep,
                CurrentStep = new LoopStepWrapper<Input, Output>
                {
                    CurrentStep = new MapStepWrapper<Mapper, Input, Output>()
                }
            }
        );
    }

    internal static UniversalBuilder<T, IEnumerable<Output>> ThenMap<T, Input, Output>(
        this UniversalBuilder<T, IEnumerable<Input>> target,
        IMapInput<Input, Output> mapper
    )
        where T : IGrain
    {
        return new UniversalBuilder<T, IEnumerable<Output>>(
            new TransformStepWrapper<IEnumerable<Input>, IEnumerable<Output>>
            {
                PreviousStep = target.ExecutionStep,
                CurrentStep = new LoopStepWrapper<Input, Output>
                {
                    CurrentStep = new MapStepWrapper<Input, Output>
                    {
                        Mapper = mapper
                    }
                }
            }
        );
    }

    internal static UniversalBuilder<T, IEnumerable<Output>> ThenFilter<T, Mapper, Output>(this UniversalBuilder<T, IEnumerable<Output>> target)
        where T : IGrain
        where Mapper : IMapInput<Output, bool>
    {
        return new UniversalBuilder<T, IEnumerable<Output>>(
            new TransformStepWrapper<IEnumerable<Output>, IEnumerable<Output>>
            {
                PreviousStep = target.ExecutionStep,
                CurrentStep = new FilterStepWrapper<Mapper, IEnumerable<Output>, Output>
                {
                    CurrentStep = new MapStepWrapper<Mapper, Output, bool>(),
                }
            }
        );
    }

    internal static UniversalBuilder<T, IEnumerable<Output>> ThenFilter<T, Output>(
        this UniversalBuilder<T, IEnumerable<Output>> target,
        IMapInput<Output, bool> mapper
    )
        where T : IGrain
    {
        return new UniversalBuilder<T, IEnumerable<Output>>(
            new TransformStepWrapper<IEnumerable<Output>, IEnumerable<Output>>
            {
                PreviousStep = target.ExecutionStep,
                CurrentStep = new FilterStepWrapper<IEnumerable<Output>, Output>
                {
                    CurrentStep = new MapStepWrapper<Output, bool>
                    {
                        Mapper = mapper
                    },
                }
            }
        );
    }

    internal static UniversalBuilder<T, IO> ThenReduce<T, Reducer, IO>(this UniversalBuilder<T, IEnumerable<IO>> target)
        where T : IGrain
        where Reducer : IReduceInputs<IO>
    {
        return new UniversalBuilder<T, IO>(
            new TransformStepWrapper<IEnumerable<IO>, IO>
            {
                PreviousStep = target.ExecutionStep,
                CurrentStep = new ReduceStepWrapper<Reducer, IO>()
            }
        );
    }

    internal static UniversalBuilder<T, IO> ThenReduce<T, IO>(
        this UniversalBuilder<T, IEnumerable<IO>> target,
        IReduceInputs<IO> reducer
    )
        where T : IGrain
    {
        return new UniversalBuilder<T, IO>(
            new TransformStepWrapper<IEnumerable<IO>, IO>
            {
                PreviousStep = target.ExecutionStep,
                CurrentStep = new ReduceStepWrapper<IO>
                {
                    Reducer = reducer
                }
            }
        );
    }
}
