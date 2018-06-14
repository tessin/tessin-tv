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
using TessinTelevisionServer.Commands;

namespace TessinTelevisionServer
{
    public class HelloRequest
    {
        [JsonProperty("hostID")]
        public HostID HostID { get; set; }
    }

    public class HostID
    {
        [JsonProperty("hostname")]
        public string Hostname { get; set; }

        [JsonProperty("serialNumber")]
        public string SerialNumber { get; set; }
    }

    public class HelloResponse
    {
        [JsonProperty("id")]
        public Guid Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("getCommandUrl")]
        public Uri GetCommandUrl { get; set; }
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

            if (command?.HostID == null)
            {
                return req.CreateResponse<Result>(ErrorCode.MissingHostID);
            }

            if (string.IsNullOrEmpty(command.HostID.Hostname))
            {
                return req.CreateResponse<Result>(ErrorCode.MissingHostname);
            }

            if (string.IsNullOrEmpty(command.HostID.SerialNumber))
            {
                return req.CreateResponse<Result>(ErrorCode.MissingSerialNumber);
            }

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

            log.Info($"TV-ID: {pi.Id}");

            var commandQueue = Storage.GetCommandQueueReference(pi.Id);

            await commandQueue.CreateIfNotExistsAsync();

            // boot commands:

            if (!string.IsNullOrEmpty(pi.GotoUrl))
            {
                Uri gotoUrl;
                if (Uri.TryCreate(pi.GotoUrl, UriKind.Absolute, out gotoUrl))
                {
                    await commandQueue.AddCommandAsync(new GotoCommand { Url = gotoUrl });
                }
                else
                {
                    log.Error($"'{pi.GotoUrl}' is not a valid absolute URL");
                }
            }

            return req.CreateResponse<Result<HelloResponse>>(new HelloResponse
            {
                Id = pi.Id,
                Name = pi.Name,
                GetCommandUrl = new Uri(req.RequestUri, $"/api/tv/{pi.Id}/commands/-/get"),
            });
        }
    }
}
