namespace Remotr;

public interface ICosmosPartitionedItem
{
    void SetPartitionKey(string partitionKey);
}
