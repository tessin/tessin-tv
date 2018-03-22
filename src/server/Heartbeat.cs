using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Tessin
{
    public static class Heartbeat
    {
        [FunctionName("Heartbeat")]
        public static async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "pi/hosts/{host}/heartbeat")]HttpRequestMessage req,
            string host,
            TraceWriter log)
        {
            var heartbeatTask = RaspberryPiManager.HeartbeatAsync(host);
            var temperatureTask = RaspberryPiManager.PostTemperatureAsync(host, await req.Content.ReadAsStringAsync());

            await heartbeatTask;
            await temperatureTask;

            return req.CreateResponse(HttpStatusCode.NoContent);
        }
    }
}
