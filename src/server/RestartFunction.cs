using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json.Linq;
using System;
using System.Threading.Tasks;
using TessinTelevisionServer.Commands;

namespace TessinTelevisionServer
{
    public static class RestartFunction
    {
        [FunctionName("RestartFunction")]
        public static async Task Run(
            [TimerTrigger("0 0 8 * * 1-5")]
            TimerInfo myTimer,
            TraceWriter log
            )
        {
            var queues = await Storage.GetCommandQueueReferences();

            int n = 0;
            foreach (var queue in queues)
            {
                await queue.AddCommandAsync(new ExecCommand { Command = "sudo shutdown -r +1" });
                n++;
            }

            log.Info($"{n} device(s) restarted"); 
        }
    }
}
