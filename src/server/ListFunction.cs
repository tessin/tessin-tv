using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace TessinTelevisionServer
{
  public class ListResponse
  {
    [JsonProperty("id")]
    public Guid? Id { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("gotoUrl")]
    public Uri GotoUrl { get; set; }

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
      var repo = new RaspberryPiRepository();

      var list = await repo.GetAll();

      var list2 = new List<ListResponse>();

      foreach (var result in list)
      {
        list2.Add(new ListResponse
        {
          Id = result.Id,
          Name = result.Name,
          GotoUrl = Utils.ParseUri(result.GotoUrl),
          Hostname = result.Hostname,
          SerialNumber = result.SerialNumber,
          PostCommandUrl = new Uri(req.RequestUri, $"/api/tv/{result.Id}/commands"),
        });
      }

      return req.CreateResponse<Result<List<ListResponse>>>(list2);
    }
  }
}
