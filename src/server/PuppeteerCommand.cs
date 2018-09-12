using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TessinTelevisionServer.Commands;

namespace TessinTelevisionServer
{
    class PuppeteerCommand : Command
    {
        [JsonProperty("commands")]
        public JArray Commands { get; set; }

        public PuppeteerCommand()
            : base("puppeteer")
        {
        }
    }
}
