using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace JK.Books.Crawlers
{
    public static class HttpClientExtensions
    {
        public static (int statusCode, string html) GetString(this HttpClient client, string url, Encoding encoding)
        {
            return GetStringAsync(client, url, encoding).GetAwaiter().GetResult();
        }

        public static (int statusCode, string html) GetString(this HttpClient client, string url, Encoding encoding,bool checkSuccessStatusCode)
        {
            return GetStringAsync(client, url, encoding, checkSuccessStatusCode).GetAwaiter().GetResult();
        }

        public static async Task<(int statusCode, string html)> GetStringAsync(this HttpClient client, string url, Encoding encoding)
        {
            return await GetStringAsync(client, url, encoding,true);
        }
        public static async Task<(int statusCode, string html)> GetStringAsync(this HttpClient client, string url, Encoding encoding,bool checkSuccessStatusCode)
        {
            var responseMessage = await client.GetAsync(url);
            int statusCode = System.Convert.ToInt32(responseMessage.StatusCode);
            if (checkSuccessStatusCode)
            {
                responseMessage.EnsureSuccessStatusCode();
            }
            var stream = await responseMessage.Content.ReadAsStreamAsync();
            using (stream)
            {
                using (StreamReader reader = new StreamReader(stream, encoding))
                {
                    var html = await reader.ReadToEndAsync();
                    return (statusCode,html);
                }
            }
        }
    }

}