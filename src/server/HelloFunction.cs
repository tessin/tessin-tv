using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace TessinTelevisionServer
{
    public class HelloRequest
    {
        [JsonProperty("version")]
        public string Version { get; set; }

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
        public Guid? Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("gotoUrl")]
        public Uri GotoUrl { get; set; }

        [JsonProperty("getCommandUrl")]
        public Uri GetCommandUrl { get; set; }

        [JsonProperty("eventsUrl")]
        public Uri EventsUrl { get; set; }

        [JsonIgnore]
        public bool HasJobs => 0 < Jobs?.Count;

        [JsonProperty("jobs")]
        public List<HelloJobResponse> Jobs { get; set; }

        public bool ShouldSerializeJobs()
        {
            return HasJobs;
        }
    }

    public class HelloJobResponse
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("cronExpression")]
        public string CronExpression { get; set; }

        [JsonProperty("command")]
        public JObject Command { get; set; }

        /// <summary>
        /// timezone from http://momentjs.com/timezone/
        /// </summary>
        [JsonProperty("timeZone")]
        public string TimeZone { get; set; }
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

            HelloRequest command;

            try
            {
                command = await req.Content.ReadAsAsync<HelloRequest>();
            }
            catch (Exception ex)
            {
                return req.CreateResponse(new Result { ErrorCode = ErrorCode.BadRequest, ErrorMessage = ex.Message });
            }

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
                            Id = Guid.NewGuid(), // once

                            Hostname = command.HostID.Hostname,
                            Version = command.Version,
                        }
                    )
                );
            }
            else
            {
                var result2 = await table.ExecuteAsync(
                    TableOperation.Merge(
                        new RaspberryPiEntity(command.HostID.SerialNumber)
                        {
                            ETag = result.Etag,

                            Hostname = command.HostID.Hostname,
                            Version = command.Version,
                        }
                    )
                );

                // patch retrieve op
                ((RaspberryPiEntity)result.Result).Hostname = command.HostID.Hostname;
                ((RaspberryPiEntity)result.Result).Version = command.Version;
            }

            var pi = (RaspberryPiEntity)result.Result;

            log.Info($"TV-ID: {pi.Id}");

            var commandQueue = Storage.GetCommandQueueReference(pi.Id);

            await commandQueue.CreateIfNotExistsAsync();

            Uri gotoUrl = null;
            if (!string.IsNullOrEmpty(pi.GotoUrl))
            {
                if (!Uri.TryCreate(pi.GotoUrl, UriKind.Absolute, out gotoUrl))
                {
                    log.Error($"'{pi.Name}' has invalid goto URL");
                }
            }

            var jobsSegment = await table.ExecuteQuerySegmentedAsync(new TableQuery<JobEntity>
            {
                FilterString = TableQuery.CombineFilters(
                    TableQuery.GenerateFilterCondition(nameof(JobEntity.PartitionKey), QueryComparisons.Equal, JobEntity.Prefix),
                    TableOperators.And,
                    TableQuery.GenerateFilterConditionForBool(nameof(JobEntity.Enabled), QueryComparisons.Equal, true)
                )
            }, null);

            var jobs = new List<HelloJobResponse>();

            foreach (var job in jobsSegment.Results)
            {
                JObject jobCommand;

                try
                {
                    jobCommand = JObject.Parse(job.Command);
                }
                catch (Exception ex)
                {
                    log.Error($"job '{job.Name}' has invalid payload. this job will not be run", ex);
                    continue;
                }

                jobs.Add(new HelloJobResponse
                {
                    Name = job.Name,
                    CronExpression = job.CronExpression,
                    Command = jobCommand,
                    TimeZone = job.TimeZone
                });
            }

            return req.CreateResponse<Result<HelloResponse>>(new HelloResponse
            {
                Id = pi.Id,
                Name = pi.Name,
                GotoUrl = gotoUrl,
                GetCommandUrl = new Uri(req.RequestUri, $"/api/tv/{pi.Id}/commands/-/get"),
                EventsUrl = new Uri(req.RequestUri, $"/api/tv/{pi.Id}/events"),
                Jobs = jobs
            });
        }
    }
}
