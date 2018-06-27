using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace TessinTelevisionServer
{
    [JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
    public enum ErrorCode
    {
        /// <summary>
        /// Success
        /// </summary>
        None = 0,

        [EnumMember(Value = "TV_BAD_REQUEST")]
        BadRequest,

        [EnumMember(Value = "TV_MISSING_HOST_ID")]
        MissingHostID,

        [EnumMember(Value = "TV_MISSING_HOSTNAME")]
        MissingHostname,

        [EnumMember(Value = "TV_MISSING_SERIAL_NUMBER")]
        MissingSerialNumber,

        [EnumMember(Value = "TV_MISSING_POP_RECEIPT")]
        MissingPopReceipt,

        /// <summary>
        /// Not an error, command queue is empty. 
        /// </summary>
        [EnumMember(Value = "TV_COMMAND_QUEUE_IS_EMPTY")]
        TvCommandQueueIsEmpty,
    }

    public class Result
    {
        public static implicit operator Result(ErrorCode errorCode)
        {
            return new Result { ErrorCode = errorCode };
        }

        [JsonProperty("errorCode", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public ErrorCode ErrorCode { get; set; }

        [JsonProperty("errorMessage", NullValueHandling = NullValueHandling.Ignore)]
        public string ErrorMessage { get; set; }

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
