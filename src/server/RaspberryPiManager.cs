using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Tessin
{
    class RaspberryPiManager
    {
        private static readonly CloudStorageAccount _storageAccount;

        static RaspberryPiManager()
        {
            _storageAccount = CloudStorageAccount.Parse(Environment.GetEnvironmentVariable("WEBSITE_CONTENTAZUREFILECONNECTIONSTRING"));
        }

        private static readonly AsyncLazy<CloudTable> _heartbeatTable = new AsyncLazy<CloudTable>(async () =>
        {
            var tableClient = _storageAccount.CreateCloudTableClient();
            var table = tableClient.GetTableReference("RaspberryPiHeartbeat");
            await table.CreateIfNotExistsAsync();
            return table;
        });

        private static readonly AsyncLazy<CloudTable> _configTable = new AsyncLazy<CloudTable>(async () =>
        {
            var tableClient = _storageAccount.CreateCloudTableClient();
            var table = tableClient.GetTableReference("RaspberryPiConfig");
            await table.CreateIfNotExistsAsync();
            return table;
        });

        private static readonly AsyncLazy<CloudTable> _temperatureTable = new AsyncLazy<CloudTable>(async () =>
        {
            var tableClient = _storageAccount.CreateCloudTableClient();
            var table = tableClient.GetTableReference("RaspberryPiTemperature");
            await table.CreateIfNotExistsAsync();
            return table;
        });

        public static async Task HeartbeatAsync(string host)
        {
            var heartbeat = new DynamicTableEntity(host, "heartbeat");

            heartbeat.Properties["HostName"] = EntityProperty.GeneratePropertyForString(host);
            heartbeat.Properties["Updated"] = EntityProperty.GeneratePropertyForDateTimeOffset(DateTimeOffset.UtcNow);

            var heartbeatTable = await _heartbeatTable;
            await heartbeatTable.ExecuteAsync(TableOperation.InsertOrMerge(heartbeat));
        }

        public static async Task PostTemperatureAsync(string host, string value)
        {
            var now = DateTimeOffset.UtcNow;
            var tick = (now.Ticks / TimeSpan.TicksPerMinute) % (1440 * 30);

            var pk = host;
            var rk = $"temp-{tick:00000}";

            var measurement = new DynamicTableEntity(pk, rk);

            measurement.Properties["Value"] = EntityProperty.GeneratePropertyForString(value);
            measurement.Properties["Updated"] = EntityProperty.GeneratePropertyForDateTimeOffset(now);

            var temperatureTable = await _temperatureTable;
            await temperatureTable.ExecuteAsync(TableOperation.InsertOrMerge(measurement));
        }

        public static async Task<List<string>> GetTemperatureAsync(string host, int min)
        {
            var table = await _temperatureTable;

            var now = DateTimeOffset.UtcNow;
            var tick = (now.Ticks / TimeSpan.TicksPerMinute) % (1440 * 30);

            if (tick >= min)
            {
                tick -= min;
            }
            else
            {
                // todo: support wrap around
            }

            var q = new TableQuery
            {
                FilterString = TableQuery.CombineFilters(
                    TableQuery.GenerateFilterCondition("PartitionKey", "eq", host),
                    "and",
                    TableQuery.GenerateFilterCondition("RowKey", "gt", "temp-")
                ),
                TakeCount = min + 1
            };

            var result = await table.ExecuteQuerySegmentedAsync(q, TableUtils.EncodeContinuationToken(host, $"temp-{tick:00000}"));

            var list = new List<string>();

            var lower = now - TimeSpan.FromSeconds(min);

            foreach (var item in result.Results)
            {
                var updated = item.Properties["Updated"].DateTimeOffsetValue;

                if (lower <= updated && updated <= now)
                {
                    list.Add(item.Properties["Value"].StringValue);
                }
            }

            return list;
        }

        public static async Task<string> GetAutostartAsync(string host)
        {
            var configTable = await _configTable;
            var result = await configTable.ExecuteAsync(TableOperation.Retrieve(host, "config"));

            var autostart = ResourceUtils.GetStringResource("autostart");

            var config = result.Result as DynamicTableEntity;
            if (config == null)
            {
                // use script as-is (default)
            }
            else
            {
                autostart = autostart.Replace("https://tessin.se", config.Properties["Url"].StringValue);
            }

            return autostart;
        }
    }
}
