using Orleans.Concurrency;
using Orleans.Runtime;

namespace Remotr;

[StatelessWorker(1)]
public abstract class IsolatedTransactionalGrain : Grain, IIncomingGrainCallFilter, IOutgoingGrainCallFilter, IGetAggregateRoot
{
    protected abstract TransactionMetadata? GetTransactionMetadata();

    protected abstract void SetTransactionMetadata(TransactionMetadata transactionMetadata);

    public abstract IAggregateRoot GetAggregate();

    public Task Invoke(IIncomingGrainCallContext context)
    {
        if (RequestContext.Get("readOnly") is bool readOnly && readOnly)
        {
            return context.Invoke();
        }


        RequestContext.AllowCallChainReentrancy();

        Guid? transactionId = RequestContext.Get("transactionId") as Guid?;
        var grainTransactionMetadata = GetTransactionMetadata();

        if (transactionId == null)
        {
            throw new Exception("All calls must be made within the context of a transaction.");
        }

        var transactionTimestamp = RequestContext.Get("transactionTimestamp") as DateTime? ?? throw new InvalidOperationException("Transactions must have a timestamp.");
        if (grainTransactionMetadata != null && transactionId == grainTransactionMetadata.TransactionId)
        {
            return context.Invoke();
        }
        else if (grainTransactionMetadata == null || transactionTimestamp > grainTransactionMetadata.TimeStamp)
        {
            SetTransactionMetadata(new TransactionMetadata
            {
                TransactionId = transactionId ?? Guid.NewGuid(),
                TimeStamp = transactionTimestamp
            });
            return context.Invoke();
        }
        else
        {
            // The timestamp of the current transaction is less than the timestamp of the transaction that this grain is currently participating in.
            throw new InvalidOperationException("Stale transaction detected.");
        }
    }

    public Task Invoke(IOutgoingGrainCallContext context)
    {
        var tmId = GetAggregate().GetGrainId().ToString();

        string? currentTmId = RequestContext.Get("aggregateRootId") as string;

        if (currentTmId == null || currentTmId != tmId)
        {
            RequestContext.Set("aggregateRootId", tmId);
        }

        return context.Invoke();
    }
}
