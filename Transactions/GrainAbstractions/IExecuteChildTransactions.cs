using Remotr;

namespace Trackr.Backend.Core.Remotr.Transactions.GrainAbstractions;

public interface IExecuteChildTransactions : IGrain
{
    Task<TOutput> Execute<TOutput>(ExecutionStep<TOutput> execution, bool interleave);
}
