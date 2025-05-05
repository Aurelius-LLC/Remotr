namespace Remotr;

public interface IGrainQueryBaseBuilder<T, Q> : IProduceUniversalBuilder<T, object>, IRunBuilder<T, Task> where T : IGrain
{
    public IGrainQueryBuilder<T, Q, Y> Ask<X, Y>() where X : IAsyncQueryHandler<T, Y>, Q;

    public IGrainQueryBuilder<T, Q, Z> Ask<X, Y, Z>(Y dto) where X : IAsyncQueryHandler<T, Y, Z>, Q;

    public IGrainQueryBuilder<T, Q, Merged> MergeSplit<Output1, Output2, Merged>
    (
        Func<IGrainQueryBaseBuilder<T, Q>, IGrainQueryBuilder<T, Q, Output1>> firstOp,
        Func<IGrainQueryBaseBuilder<T, Q>, IGrainQueryBuilder<T, Q, Output2>> secondOp,
        IMergeInputs<Output1, Output2, Merged> merger
    );

    public IGrainQueryBuilder<T, Q, Merged> MergeSplit<Merger, Output1, Output2, Merged>
    (
        Func<IGrainQueryBaseBuilder<T, Q>, IGrainQueryBuilder<T, Q, Output1>> firstOp,
        Func<IGrainQueryBaseBuilder<T, Q>, IGrainQueryBuilder<T, Q, Output2>> secondOp
    ) where Merger : IMergeInputs<Output1, Output2, Merged>;


    public IGrainQueryBuilder<T, Q, IEnumerable<Output>> ForEach<Input, Output>
    (
        IEnumerable<Input> inputs,
        Func<IGrainQueryBuilder<T, Q, Input>, IGrainQueryBuilder<T, Q, Output>> operation
    );
}
