using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using System.Threading.Tasks;
using TessinTelevisionServer.Commands;

namespace TessinTelevisionServer
{
    public static class DisplayOffFunction
    {
        [FunctionName("DisplayOffFunction")]
        public static async Task Run(
            [TimerTrigger("0 0 19 * * *")]
            TimerInfo myTimer,
            TraceWriter log
            )
        {
            var queues = await Storage.GetCommandQueueReferences();

            int n = 0;
            foreach (var queue in queues)
            {
                await queue.AddCommandAsync(new ExecCommand { Command = "sudo vcgencmd display_power 0" });
                n++;
            }

            log.Info($"{n} displays powered off");
        }
    }

    public static class DisplayOnFunction
    {
        [FunctionName("DisplayOnFunction")]
        public static async Task Run(
            [TimerTrigger("0 0 7 * * *")]
            TimerInfo myTimer,
            TraceWriter log
            )
        {
            var queues = await Storage.GetCommandQueueReferences();

            int n = 0;
            foreach (var queue in queues)
            {
                await queue.AddCommandAsync(new ExecCommand { Command = "sudo vcgencmd display_power 1" });
                n++;
            }

            log.Info($"{n} displays powered on");
        }
    }
}
