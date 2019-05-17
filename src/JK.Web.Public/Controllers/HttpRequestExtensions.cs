using Microsoft.AspNetCore.Http;
using System.IO;
using System.Text;

namespace JK.Web.Public.Controllers
{
    public static class HttpRequestExtensions
    {
        public static string GetBodyContent(this HttpRequest httpRequest)
        {
            using (var buffer = new MemoryStream())
            {
                httpRequest.Body.CopyTo(buffer);
                buffer.Position = 0;
                var bytes = buffer.ToArray();
                return Encoding.UTF8.GetString(bytes);
            }
        }
    }
}
