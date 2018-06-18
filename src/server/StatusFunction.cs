using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;

namespace TessinTelevisionServer
{
    public static class StatusFunction
    {
        [FunctionName("StatusFunction")]
        public static async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = "tv/{id}/status")]
            HttpRequestMessage req,
            string id,
            TraceWriter log
            )
        {
            // get  - status report
            // post - report status

            if (req.Method == HttpMethod.Get)
            {
                // get status report

                return req.CreateResponse<Result>(ErrorCode.None);
            }

            if (req.Method == HttpMethod.Post)
            {
                // report status

                return req.CreateResponse<Result>(ErrorCode.None);
            }

            return req.CreateResponse(HttpStatusCode.MethodNotAllowed);
        }
    }
}
