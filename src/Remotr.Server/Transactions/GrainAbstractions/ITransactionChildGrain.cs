using Trackr.Backend.Core.Remotr.Transactions.GrainAbstractions;

namespace Remotr;

public interface IAggregateEntity<T> :
    IExecuteChildTransactions,
    IHaveTransactionOperations,
    IGrainWithStringKey
        where T : new()
{
    Task HandleCallback(Guid callbackId);
}