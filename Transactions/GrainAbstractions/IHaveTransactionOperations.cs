namespace Remotr;

public interface IHaveTransactionOperations : IGrain
{
    Task<TransactionOperations> GetTransactionOperations();
}
