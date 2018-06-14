using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TessinTelevisionServer.Commands
{
    class ExecCommand : Command
    {
        [JsonProperty("command")]
        public string Command { get; set; }

        public ExecCommand()
            : base("exec")
        {
        }
    }
}
