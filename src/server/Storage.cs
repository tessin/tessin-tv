using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TessinTelevisionServer
{
    public static class Storage
    {
        private static readonly Lazy<CloudStorageAccount> _storageAccount;

        public static AsyncLazy<CloudTable> Table { get; }

        static Storage()
        {
            _storageAccount = new Lazy<CloudStorageAccount>(
                () => CloudStorageAccount.Parse(
                    Environment.GetEnvironmentVariable("AzureWebJobsStorage", EnvironmentVariableTarget.Process)
                )
            );

            Table = Async.Lazy(async () =>
            {
                var tableClient = _storageAccount.Value.CreateCloudTableClient();

                var table = tableClient.GetTableReference("TessinTV");
                await table.CreateIfNotExistsAsync();
                return table;
            });
        }

        public static CloudQueue GetCommandQueueReference(Guid? id)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }
            var queueClient = _storageAccount.Value.CreateCloudQueueClient();
            var queue = queueClient.GetQueueReference(FormattableString.Invariant($"tv-{id}"));
            return queue;
        }

        public static async Task<List<CloudQueue>> GetCommandQueueReferences()
        {
            var queueClient = _storageAccount.Value.CreateCloudQueueClient();

            var segment = await queueClient.ListQueuesSegmentedAsync("tv-", null);

            return new List<CloudQueue>(segment.Results);
        }
    }
}
