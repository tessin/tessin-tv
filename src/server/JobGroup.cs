using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TessinTelevisionServer
{
    public class JobEntity : TableEntity
    {
        public const string Prefix = "job";

        public JobEntity()
        {
        }

        public JobEntity(string jobName)
        {
            PartitionKey = Prefix;
            RowKey = Prefix + "-" + jobName;
            Name = jobName;
        }

        public string Name { get; set; }

        public string CronExpression { get; set; }

        /// <summary>
        /// JSON
        /// </summary>
        public string Command { get; set; }

        /// <summary>
        /// timezone from http://momentjs.com/timezone/
        /// </summary>
        public string TimeZone { get; set; }

        public bool Enabled { get; set; }
    }
}
