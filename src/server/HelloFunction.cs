using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;

namespace TessinTelevisionServer
{
    public class HelloRequest
    {
        [JsonProperty("hostID")]
        public HostID HostID { get; set; } = new HostID();
    }

    public class HostID
    {
        [JsonProperty("hostname")]
        public string Hostname { get; set; }

        [JsonProperty("serialNumber")]
        public string SerialNumber { get; set; }
    }

    public static class HelloFunction
    {
        [FunctionName("HelloFunction")]
        public static async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "hello")]
            HttpRequestMessage req,
            TraceWriter log
            )
        {
            log.Info($"<<<<: {await req.Content.ReadAsStringAsync()}");

            var command = await req.Content.ReadAsAsync<HelloRequest>();

            log.Info($"HostID: {command.HostID.Hostname}, {command.HostID.SerialNumber}");

            var table = await Storage.Table;

            var result = await table.ExecuteAsync(TableOperation.Retrieve<RaspberryPiEntity>("pi", command.HostID.SerialNumber));
            if (result.HttpStatusCode == 404)
            {
                result = await table.ExecuteAsync(
                    TableOperation.Insert(
                        new RaspberryPiEntity(command.HostID.SerialNumber)
                        {
                            Hostname = command.HostID.Hostname,
                            Id = Guid.NewGuid()
                        }
                    )
                );
            }

            var pi = (RaspberryPiEntity)result.Result;

            log.Info($"{pi.Id}");

            return req.CreateResponse<Result>(ErrorCode.None);
        }
    }
}
