using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace TessinTelevisionServer
{
    public class CommandResponse
    {
        [JsonProperty("command")]
        public JObject Command { get; set; }

        [JsonProperty("completeUrl")]
        public Uri CompleteUrl { get; set; }
    }

    public static class CommandGetFunction
    {
        [FunctionName("CommandFunction")]
        public static async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "tv/{id}/commands/-/get")]
            HttpRequestMessage req,
            string id,
            TraceWriter log
        )
        {
            var id2 = new Guid(id);

            var queue = Storage.GetCommandQueueReference(id2);

            _GetMessage:

            var msg = await queue.GetMessageAsync(TimeSpan.FromMinutes(2), new Microsoft.WindowsAzure.Storage.Queue.QueueRequestOptions(), null);
            if (msg == null)
            {
                return req.CreateResponse<Result>(ErrorCode.TvCommandQueueIsEmpty);
            }

            if (!(msg.DequeueCount <= 1))
            {
                // dead letter
                await queue.DeleteMessageAsync(msg);
                goto _GetMessage;
            }

            var command = JObject.Parse(msg.AsString);

            return req.CreateResponse(new Result<CommandResponse>
            {
                Payload = new CommandResponse
                {
                    Command = command,
                    CompleteUrl = new Uri(req.RequestUri, $"/api/tv/{id}/commands/{Uri.EscapeDataString(msg.Id)}/delete?popReceipt={Uri.EscapeDataString(msg.PopReceipt)}")
                }
            });
        }
    }
}
