using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using TessinTelevisionServer.Commands;

namespace TessinTelevisionServer
{
    public static class CommandPostFunction
    {
        [FunctionName("CommandPostFunction")]
        public static async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "tv/{id}/commands")]
            HttpRequestMessage req,
            string id,
            TraceWriter log)
        {
            var command = await req.Content.ReadAsAsync<JObject>();

            var queue = Storage.GetCommandQueueReference(new Guid(id));

            // todo: this will fail if the device has not issued it's first hello yet

            await queue.AddCommandAsync(command);

            return req.CreateResponse<Result>(ErrorCode.None);
        }
    }
}
