
using Orleans.Concurrency;

namespace Remotr;

public interface IExecuteEntityTransactions : IGrain
{
    [AlwaysInterleave]
    Task<TOutput> ExecuteInterleaving<TOutput>(ExecutionStep<TOutput> execution);
    
    Task<TOutput> ExecuteNotInterleaving<TOutput>(ExecutionStep<TOutput> execution);
}
