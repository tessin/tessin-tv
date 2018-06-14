using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TessinTelevisionServer.Commands
{
    static class CommandUtils
    {
        public static async Task AddCommandAsync(this CloudQueue queue, Command command)
        {
            var json = JsonConvert.SerializeObject(command);

            await queue.AddMessageAsync(new CloudQueueMessage(json));
        }
    }
}
