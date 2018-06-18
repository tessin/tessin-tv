using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using System;
using System.Threading.Tasks;
using TessinTelevisionServer.Commands;

namespace TessinTelevisionServer
{
    public static class StandupFunction
    {
        [FunctionName("StandupFunction")]
        public static async Task Run(
            [TimerTrigger("0 57 8 * * 1-5")]
            TimerInfo myTimer,
            TraceWriter log
            )
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
        [FunctionName("StanddownFunction")]
        public static async Task Run(
            [TimerTrigger("0 14 9 * * 1-5")]
            TimerInfo myTimer,
            TraceWriter log
            )
        {
            var repo = new RaspberryPiRepository();

            var list = await repo.GetAll();

            foreach (var pi in list)
            {
                var queue = Storage.GetCommandQueueReference(pi.Id);

                if (!string.IsNullOrEmpty(pi.GotoUrl))
                {
                    Uri gotoUrl;
                    if (Uri.TryCreate(pi.GotoUrl, UriKind.Absolute, out gotoUrl))
                    {
                        await queue.AddCommandAsync(new GotoCommand { Url = gotoUrl });
                    }
                    else
                    {
                        await queue.AddCommandAsync(new GotoCommand { Url = new Uri("splash:") });
                    }
                }
            }
        }
    }
}
