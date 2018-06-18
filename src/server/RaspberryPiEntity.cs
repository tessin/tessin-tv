using Microsoft.WindowsAzure.Storage.Table;
using System;

namespace TessinTelevisionServer
{
    public class RaspberryPiEntity : TableEntity
    {
        public static readonly string Prefix = "pi";

        public RaspberryPiEntity()
        {
        }

        public RaspberryPiEntity(string serialNumber)
        {
            this.PartitionKey = Prefix;
            this.RowKey = serialNumber;

            this.SerialNumber = serialNumber;
        }

        public string Hostname { get; set; }
        public string SerialNumber { get; set; }

        public Guid? Id { get; set; }
        public string Name { get; set; }

        public string GotoUrl { get; set; }
    }
}
