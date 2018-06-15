using Microsoft.WindowsAzure.Storage.Table;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TessinTelevisionServer
{
    public class RaspberryPiRepository
    {
        public async Task<List<RaspberryPiEntity>> GetAll()
        {
            var table = await Storage.Table;

            var segment = await table.ExecuteQuerySegmentedAsync(new TableQuery<RaspberryPiEntity>
            {
                FilterString = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, RaspberryPiEntity.Prefix)
            }, null);

            return segment.Results;
        }
    }
}
