using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TessinTelevisionServer.Commands;

namespace TessinTelevisionServer
{
    public static class StandupFunction
    {
        [FunctionName("StandupFunction")]
        public static async Task Run([TimerTrigger("0 57 8 * * *")]TimerInfo myTimer, TraceWriter log)
        {
            var standupUrl = new Uri("https://tessinraspi89f4.blob.core.windows.net/static/standup.htm");

            var queues = await Storage.GetCommandQueueReferences();

            foreach (var queue in queues)
            {
                await queue.AddCommandAsync(new GotoCommand { Url = standupUrl });
            }
        }
    }

    public static class StanddownFunction
    {
        [FunctionName("StandupFunction")]
        public static async Task Run([TimerTrigger("0 14 9 * * *")]TimerInfo myTimer, TraceWriter log)
        {
            var table = await Storage.Table;

            var segment = await table.ExecuteQuerySegmentedAsync(new TableQuery<RaspberryPiEntity>
            {
                FilterString = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "pi")
            }, null);

            var list = new List<ListResponse>();

            foreach (var pi in segment.Results)
            {
                var queue = Storage.GetCommandQueueReference(pi.Id);

                if (!string.IsNullOrEmpty(pi.GotoUrl))
                {
                    Uri gotoUrl;
                    if (Uri.TryCreate(pi.GotoUrl, UriKind.Absolute, out gotoUrl))
                    {
                        await queue.AddCommandAsync(new GotoCommand { Url = gotoUrl });
                    }
                }
            }
        }
    }
}
