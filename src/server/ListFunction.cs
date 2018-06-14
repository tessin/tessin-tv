using System;
using System.Collections.Generic;
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
    public class ListResponse
    {
        [JsonProperty("id")]
        public Guid Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("hostname")]
        public string Hostname { get; set; }

        [JsonProperty("serialNumber")]
        public string SerialNumber { get; set; }

        [JsonProperty("postCommandUrl")]
        public Uri PostCommandUrl { get; set; }
    }

    public static class ListFunction
    {
        [FunctionName("ListFunction")]
        public static async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "list")]
            HttpRequestMessage req,
            TraceWriter log
            )
        {
            var table = await Storage.Table;

            var segment = await table.ExecuteQuerySegmentedAsync(new TableQuery<RaspberryPiEntity>
            {
                FilterString = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "pi")
            }, null);

            var list = new List<ListResponse>();

            foreach (var result in segment.Results)
            {
                list.Add(new ListResponse
                {
                    Id = result.Id,
                    Name = result.Name,
                    Hostname = result.Hostname,
                    SerialNumber = result.SerialNumber,
                    PostCommandUrl = new Uri(req.RequestUri, $"/api/tv/{result.Id}/commands")
                });
            }

            return req.CreateResponse<Result<List<ListResponse>>>(list);
        }
    }
}
