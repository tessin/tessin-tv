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
        public JobEntity()
        {
        }

        public JobEntity(string jobName)
        {
            PartitionKey = "job";
            RowKey = "job-" + jobName;
        }

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
