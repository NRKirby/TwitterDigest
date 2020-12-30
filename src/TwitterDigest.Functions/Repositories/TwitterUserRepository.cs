using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos.Table;
using TwitterDigest.Functions.Entities;

namespace TwitterDigest.Functions.Repositories
{
    public class TwitterUserRepository
    {
        private readonly CloudTable _table;

        public TwitterUserRepository(string connectionString, string tableReference)
        {
            var account = CloudStorageAccount.Parse(connectionString);
            var tableClient = account.CreateCloudTableClient();
            _table = tableClient.GetTableReference(tableReference);
        }

        public async Task<IEnumerable<TwitterUser>> List(string partitionKey)
        {
            TableContinuationToken token = null;
            var query = new TableQuery<TwitterUser>()
                .Where($"PartitionKey eq '{partitionKey}'");
            var result = await _table.ExecuteQuerySegmentedAsync(query, null);

            var entities = new List<TwitterUser>();
            if (!result.Results.Any())
            {
                return entities;
            }

            do
            {
                var seg = await _table.ExecuteQuerySegmentedAsync(query, token);
                token = seg.ContinuationToken;
                entities.AddRange(seg);

            } while (token != null);

            return entities;
        }
    }
}
