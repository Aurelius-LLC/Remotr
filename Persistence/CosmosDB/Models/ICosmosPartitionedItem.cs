namespace Remotr;

internal interface ICosmosPartitionedItem
{
    void SetPartitionKey(string partitionKey);
}
