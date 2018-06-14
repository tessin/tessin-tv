using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TessinTelevisionServer
{
    public class RaspberryPiEntity : TableEntity
    {
        public RaspberryPiEntity()
        {
        }

        public RaspberryPiEntity(string serialNumber)
        {
            this.PartitionKey = "pi";
            this.RowKey = serialNumber;

            this.SerialNumber = serialNumber;
        }

        public string Hostname { get; set; }
        public string SerialNumber { get; set; }

        public Guid Id { get; set; }
        public string Name { get; set; }

        public string GotoUrl { get; set; }
    }
}
