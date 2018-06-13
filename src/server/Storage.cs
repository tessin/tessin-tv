using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;

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
    }
}
