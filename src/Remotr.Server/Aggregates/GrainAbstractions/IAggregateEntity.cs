

namespace Remotr;

public interface IAggregateEntity<T> :
    IExecuteEntityTransactions,
    IHaveTransactionOperations,
    IGrainWithStringKey
        where T : new()
{
    Task HandleCallback(Guid callbackId);
}