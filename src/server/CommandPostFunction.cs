using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Threading.Tasks;

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
            var id2 = new Guid(id);

            var command = await req.Content.ReadAsAsync<JObject>();

            var queue = Storage.GetCommandQueueReference(id2);

            // todo: this can fail if the device has not issued it's hello yet

            await queue.AddMessageAsync(new CloudQueueMessage(JsonConvert.SerializeObject(command)));

            return req.CreateResponse<Result>(ErrorCode.None);
        }
    }
}
