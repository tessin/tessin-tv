using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Tessin
{
    public static class GetTemperature
    {
        [FunctionName("GetTemperature")]
        public static async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "pi/hosts/{host}/temp")]HttpRequestMessage req,
            string host,
            TraceWriter log)
        {
            var parameters = req.RequestUri.ParseQueryString();
            var min = Convert.ToInt32(parameters.Get("min") ?? "60");
            var list = await RaspberryPiManager.GetTemperatureAsync(host, min);
            return req.CreateResponse(HttpStatusCode.OK, list);
        }
    }
}
