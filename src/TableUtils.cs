using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Text;

namespace Tessin
{
    static class TableUtils
    {
        public static TableContinuationToken EncodeContinuationToken(string pk, string rk)
        {
            return new TableContinuationToken
            {
                NextPartitionKey = EncodeContinuationTokenComponent(pk),
                NextRowKey = EncodeContinuationTokenComponent(rk),
                TargetLocation = StorageLocation.Primary
            };
        }

        private static string EncodeContinuationTokenComponent(string k)
        {
            var s = Convert.ToBase64String(Encoding.UTF8.GetBytes(k)).Replace('=', '-');
            return $"1!{s.Length}!{s}";
        }
    }
}
