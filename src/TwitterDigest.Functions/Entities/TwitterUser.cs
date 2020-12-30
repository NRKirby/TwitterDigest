using Microsoft.Azure.Cosmos.Table;

namespace TwitterDigest.Functions.Entities
{
    public class TwitterUser : TableEntity
    {
        public TwitterUser()
        {
            
        }

        public TwitterUser(string partitionKey, string rowKey)
        {
            PartitionKey = partitionKey;
            RowKey = rowKey;
        }
    }
}
