using Newtonsoft.Json;

namespace TessinTelevisionServer
{
    [JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
    public enum ErrorCode
    {
        /// <summary>
        /// Success
        /// </summary>
        None = 0,
    }

    public class Result
    {
        public static implicit operator Result(ErrorCode errorCode)
        {
            return new Result { ErrorCode = errorCode };
        }

        [JsonProperty("errorCode", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public ErrorCode ErrorCode { get; set; }

        [JsonProperty("success", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public bool Success => ErrorCode == 0;

        [JsonProperty("hasError", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public bool HasError => ErrorCode != 0;
    }

    public class Result<T> : Result
        where T : class
    {
        public static implicit operator Result<T>(ErrorCode errorCode)
        {
            return new Result<T> { ErrorCode = errorCode };
        }

        public static implicit operator Result<T>(T payload)
        {
            return new Result<T> { Payload = payload };
        }

        [JsonProperty("payload", NullValueHandling = NullValueHandling.Ignore)]
        public T Payload { get; set; }
    }
}
