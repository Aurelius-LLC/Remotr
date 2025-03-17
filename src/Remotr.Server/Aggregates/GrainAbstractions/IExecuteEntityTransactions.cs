
namespace Remotr;

public interface IExecuteEntityTransactions : IGrain
{
    Task<TOutput> Execute<TOutput>(ExecutionStep<TOutput> execution, bool interleave);
}
