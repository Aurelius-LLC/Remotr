namespace Remotr;

internal class UniversalBuilder<T, K> where T : IGrain
{
    internal ExecutionStep<K> ExecutionStep;

    internal UniversalBuilder(ExecutionStep<K> BaseExecutionStep)
    {
        ExecutionStep = BaseExecutionStep;
    }

    private string ExtendName(Type handlerName)
    {
        return string.IsNullOrEmpty(ExecutionStep.Name) ? handlerName.Name : $"{ExecutionStep.Name}.{handlerName.Name}";
    }

    internal UniversalBuilder<T, Y> Ask<X, Y>() where X : IAsyncQueryHandler<T, Y>
    {
        return new UniversalBuilder<T, Y>(
            new SimpleStepAsyncWrapper<K, Y>
            {
                Name = ExtendName(typeof(X)),
                PreviousStep = ExecutionStep,
                CurrentStep = new AsyncQueryWrapper<T, X, Y>()
            }
        );
    }

    internal UniversalBuilder<T, Z> Ask<X, Y, Z>(Y dto) where X : IAsyncQueryHandler<T, Y, Z>
    {
        return new UniversalBuilder<T, Z>(
            new SimpleStepWithInputAsyncWrapper<K, Y, Z>
            {
                Name = ExtendName(typeof(X)),
                PreviousStep = ExecutionStep,
                StepInput = dto,
                CurrentStep = new AsyncQueryWrapper<T, X, Y, Z>()
            }
        );
    }

    internal UniversalBuilder<T, Y> ThenAsk<X, Y>() where X : IAsyncQueryHandler<T, K, Y>
    {
        return new UniversalBuilder<T, Y>(
            new TransformStepAsyncWrapper<K, Y>
            {
                Name = ExtendName(typeof(X)),
                PreviousStep = ExecutionStep,
                CurrentStep = new AsyncQueryWrapper<T, X, K, Y>()
            }
        );
    }

    internal virtual UniversalBuilder<T, object> Tell<X>() where X : IAsyncCommandHandler<T>
    {
        return new UniversalBuilder<T, object>(
            new SimpleStepAsyncWrapper<K, object>
            {
                Name = ExtendName(typeof(X)),
                PreviousStep = ExecutionStep,
                CurrentStep = new AsyncCommandWrapper<T, X>()
            }
        );
    }

    internal UniversalBuilder<T, P> Tell<X, P>() where X : IAsyncCommandHandler<T, P>
    {
        return new UniversalBuilder<T, P>(
            new SimpleStepAsyncWrapper<K, P>
            {
                Name = ExtendName(typeof(X)),
                PreviousStep = ExecutionStep,
                CurrentStep = new AsyncCommandWrapper<T, X, P>()
            }
        );
    }

    internal UniversalBuilder<T, P> Tell<X, Y, P>(Y dto) where X : IAsyncCommandHandler<T, Y, P>
    {
        return new UniversalBuilder<T, P>(
            new SimpleStepWithInputAsyncWrapper<K, Y, P>
            {
                Name = ExtendName(typeof(X)),
                PreviousStep = ExecutionStep,
                CurrentStep = new AsyncCommandWrapper<T, X, Y, P>(),
                StepInput = dto
            }
        );
    }


    internal UniversalBuilder<T, Y> ThenTell<X, Y>() where X : IAsyncCommandHandler<T, K, Y>
    {
        return new UniversalBuilder<T, Y>(
            new TransformStepAsyncWrapper<K, Y>
            {
                Name = ExtendName(typeof(X)),
                PreviousStep = ExecutionStep,
                CurrentStep = new AsyncCommandWrapper<T, X, K, Y>()
            }
        );
    }


    internal UniversalBuilder<T, IEnumerable<Z>> ForEach<X, Y, Z>(IEnumerable<Y> dto)
        where X : IAsyncCommandHandler<T, Y, Z>
    {
        return new UniversalBuilder<T, IEnumerable<Z>>(
            new SimpleStepWithInputAsyncWrapper<K, IEnumerable<Y>, IEnumerable<Z>>
            {
                Name = ExtendName(typeof(X)),
                PreviousStep = ExecutionStep,
                CurrentStep = new LoopStepAsyncWrapper<Y, Z>
                {
                    CurrentStep = new AsyncCommandWrapper<T, X, Y, Z>(),
                },
                StepInput = dto,
            }
        );
    }

    internal UniversalBuilder<T, IEnumerable<Output>> ForEachSplit<Input, Output>(
        IEnumerable<Input> dto,
        NeedsPropStep<Input> bridgeStep,
        IProduceUniversalBuilder<T, Output> outputer
    )
    {

        var builder = outputer.Builder.ExecutionStep;

        return new UniversalBuilder<T, IEnumerable<Output>>(
            new SimpleStepWithInputAsyncWrapper<K, IEnumerable<Input>, IEnumerable<Output>>
            {
                PreviousStep = ExecutionStep,
                StepInput = dto,
                CurrentStep = new LoopStepAsyncWrapper<Input, Output>
                {
                    CurrentStep = new PropBridgeStepWrapper<Input, Output>
                    {
                        BridgeStep = bridgeStep,
                        CurrentStep = builder
                    }
                }
            }
        );
    }


    internal UniversalBuilder<T, Y> ThenMap<X, Y>() where X : IMapInput<K, Y>
    {
        return new UniversalBuilder<T, Y>(
            new TransformStepWrapper<K, Y>
            {
                PreviousStep = ExecutionStep,
                CurrentStep = new MapStepWrapper<X, K, Y>()
            }
        );
    }

    internal UniversalBuilder<T, Merged> MergeSplit<Output1, Output2, Merged>(
        IProduceUniversalBuilder<T, Output1> firstOutputer,
        IProduceUniversalBuilder<T, Output2> secondOutputer,
        IMergeInputs<Output1, Output2, Merged> merger
    )
    {
        var builder1 = firstOutputer.Builder.ExecutionStep;
        var builder2 = secondOutputer.Builder.ExecutionStep;
        return new UniversalBuilder<T, Merged>(
            new SimpleStepAsyncWrapper<K, Merged>
            {
                PreviousStep = ExecutionStep,
                CurrentStep = new MergeStepsWrapper<Output1, Output2, Merged>
                {
                    Step1 = builder1,
                    Step2 = builder2,
                    Merger = merger
                }
            }
        );
    }

    internal UniversalBuilder<T, Merged> MergeSplit<Merger, Output1, Output2, Merged>(
        IProduceUniversalBuilder<T, Output1> firstOutputer,
        IProduceUniversalBuilder<T, Output2> secondOutputer
    )
        where Merger : IMergeInputs<Output1, Output2, Merged>
    {
        var builder1 = firstOutputer.Builder.ExecutionStep;
        var builder2 = secondOutputer.Builder.ExecutionStep;
        return new UniversalBuilder<T, Merged>(
            new SimpleStepAsyncWrapper<K, Merged>
            {
                PreviousStep = ExecutionStep,
                CurrentStep = new MergeStepsWrapper<Merger, Output1, Output2, Merged>
                {
                    Step1 = builder1,
                    Step2 = builder2
                }
            }
        );
    }

    internal UniversalBuilder<T, Merged> MergeSplit<Output1, Output2, Merged>(
        NeedsPropStep<K> bridgeStep,
        IProduceUniversalBuilder<T, Output1> firstOutputer,
        IProduceUniversalBuilder<T, Output2> secondOutputer,
        IMergeInputs<Output1, Output2, Merged> merger
    )
    {
        var builder1 = firstOutputer.Builder.ExecutionStep;
        var builder2 = secondOutputer.Builder.ExecutionStep;
        return new UniversalBuilder<T, Merged>(
            new TransformStepAsyncWrapper<K, Merged>
            {
                PreviousStep = ExecutionStep,
                CurrentStep = new PropBridgeStepWrapper<K, Merged>
                {
                    BridgeStep = bridgeStep,
                    CurrentStep = new MergeStepsWrapper<Output1, Output2, Merged>
                    {
                        Step1 = builder1,
                        Step2 = builder2,
                        Merger = merger
                    }
                }
            }
        );
    }

    internal UniversalBuilder<T, Merged> MergeSplit<Merger, Output1, Output2, Merged>(
        NeedsPropStep<K> bridgeStep,
        IProduceUniversalBuilder<T, Output1> firstOutputer,
        IProduceUniversalBuilder<T, Output2> secondOutputer
    )
        where Merger : IMergeInputs<Output1, Output2, Merged>
    {
        var builder1 = firstOutputer.Builder.ExecutionStep;
        var builder2 = secondOutputer.Builder.ExecutionStep;
        return new UniversalBuilder<T, Merged>(
            new TransformStepAsyncWrapper<K, Merged>
            {
                PreviousStep = ExecutionStep,
                CurrentStep = new PropBridgeStepWrapper<K, Merged>
                {
                    BridgeStep = bridgeStep,
                    CurrentStep = new MergeStepsWrapper<Merger, Output1, Output2, Merged>
                    {
                        Step1 = builder1,
                        Step2 = builder2
                    }
                }
            }
        );
    }
}
