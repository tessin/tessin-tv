using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Tessin
{
    public static class GetAutostart
    {
        private static readonly UTF8Encoding _utf8 = new UTF8Encoding(false, false);

        [FunctionName("GetAutostart")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = "pi/hosts/{host}/autostart")]HttpRequestMessage req,
            string host,
            TraceWriter log)
        {
            var multipart = await req.Content.ReadAsMultipartAsync();
            var text = StringUtils.NormalizeLineEndings(await multipart.Contents.Single().ReadAsStringAsync());
            var autostart = await RaspberryPiManager.GetAutostartAsync(host);
            if (text != autostart)
            {
                var res = req.CreateResponse(HttpStatusCode.OK);
                res.Content = new StringContent(autostart, _utf8, "text/plain");
                return res;
            }
            return req.CreateResponse(HttpStatusCode.BadRequest);
        }
    }
}
