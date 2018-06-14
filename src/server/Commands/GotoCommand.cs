using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TessinTelevisionServer.Commands
{
    class GotoCommand : Command
    {
        [JsonProperty("url")]
        public Uri Url { get; set; }

        public GotoCommand()
            : base("goto")
        {
        }
    }
}
