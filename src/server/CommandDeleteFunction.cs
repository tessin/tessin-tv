using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace TessinTelevisionServer
{
    public static class CommandDeleteFunction
    {
        [FunctionName("CommandDeleteFunction")]
        public static async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "tv/{id}/commands/{messageId}/delete")]
            HttpRequestMessage req,
            string id,
            string messageId,
            TraceWriter log
            )
        {
            var id2 = new Guid(id);

            var query = req.RequestUri.ParseQueryString();

            var popReceipt = query["popReceipt"];

            var queue = Storage.GetCommandQueueReference(id2);

            await queue.DeleteMessageAsync(messageId, popReceipt);

            return req.CreateResponse<Result>(ErrorCode.None);
        }
    }
}
