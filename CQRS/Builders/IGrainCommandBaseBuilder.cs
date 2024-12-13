namespace Remotr;

public interface IGrainCommandBaseBuilder<T, C, Q> : IProduceUniversalBuilder<T, object>, IRunBuilder<T, Task> where T : IGrain
{
    public IGrainCommandBuilder<T, C, Q, Y> Ask<X, Y>() where X : IAsyncQueryHandler<T, Y>, Q;

    public IGrainCommandBuilder<T, C, Q, Z> Ask<X, Y, Z>(Y dto) where X : IAsyncQueryHandler<T, Y, Z>, Q;

    public IGrainCommandBaseBuilder<T, C, Q> Tell<X>() where X : IAsyncCommandHandler<T>, C;

    public IGrainCommandBuilder<T, C, Q, Y> Tell<X, Y>() where X : IAsyncCommandHandler<T, Y>, C;

    public IGrainCommandBuilder<T, C, Q, Z> Tell<X, Y, Z>(Y dto) where X : IAsyncCommandHandler<T, Y, Z>, C;

    public IGrainCommandBuilder<T, C, Q, IEnumerable<Output>> TellForEach<Handler, Input, Output>
    (
        IEnumerable<Input> inputs
    ) where Handler : IAsyncCommandHandler<T, Input, Output>, C;

    public IGrainCommandBuilder<T, C, Q, Merged> MergeSplit<Output1, Output2, Merged>
    (
        Func<IGrainCommandBaseBuilder<T, C, Q>, IGrainCommandBuilder<T, C, Q, Output1>> firstOp,
        Func<IGrainCommandBaseBuilder<T, C, Q>, IGrainCommandBuilder<T, C, Q, Output2>> secondOp,
        IMergeInputs<Output1, Output2, Merged> merger
    );

    public IGrainCommandBuilder<T, C, Q, Merged> MergeSplit<Merger, Output1, Output2, Merged>
    (
        Func<IGrainCommandBaseBuilder<T, C, Q>, IGrainCommandBuilder<T, C, Q, Output1>> firstOp,
        Func<IGrainCommandBaseBuilder<T, C, Q>, IGrainCommandBuilder<T, C, Q, Output2>> secondOp
    ) where Merger : IMergeInputs<Output1, Output2, Merged>;

    public IGrainCommandBuilder<T, C, Q, IEnumerable<Output>> ForEachSplit<Input, Output>
    (
        IEnumerable<Input> inputs,
        Func<IGrainCommandBuilder<T, C, Q, Input>, IGrainCommandBuilder<T, C, Q, Output>> operation
    );
}
