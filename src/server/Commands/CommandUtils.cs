using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TessinTelevisionServer.Commands
{
    static class CommandUtils
    {
        public static Task AddCommandAsync(this CloudQueue queue, Command command)
        {
            return queue.AddCommandAsync(JObject.FromObject(command));
        }

        public static async Task AddCommandAsync(this CloudQueue queue, JObject command)
        {
            var json = JsonConvert.SerializeObject(command);

            await queue.AddMessageAsync(
                new CloudQueueMessage(json),
                timeToLive: TimeSpan.FromMinutes(15),
                initialVisibilityDelay: null,
                options: null,
                operationContext: null
            );
        }
    }
}
