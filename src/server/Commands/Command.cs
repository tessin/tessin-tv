using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TessinTelevisionServer.Commands
{
    abstract class Command
    {
        [JsonProperty("type")]
        public string Type { get; }

        protected Command(string type)
        {
            this.Type = type;
        }
    }
}
