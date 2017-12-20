using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Tessin
{
    public static class Tick
    {
        private static readonly UTF8Encoding _utf8 = new UTF8Encoding(false, false);

        [FunctionName("Tick")]
        public static HttpResponseMessage Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = "pi/hosts/{host}/tick")]HttpRequestMessage req,
            string host,
            TraceWriter log)
        {
            var res = req.CreateResponse(HttpStatusCode.OK);
            res.Content = new StringContent(ResourceUtils.GetStringResource("tock.sh"), _utf8, "text/plain");
            return res;
        }

    }
}
