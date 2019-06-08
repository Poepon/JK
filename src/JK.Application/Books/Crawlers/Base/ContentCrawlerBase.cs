using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using JK.Books.Dto;

namespace JK.Books.Crawlers
{
    public abstract class ContentCrawlerBase : IContentCrawler
    {
        protected readonly HttpClient _client;
        public ContentCrawlerBase(IHttpClientFactory httpClientFactory)
        {
            _client = httpClientFactory.CreateClient(Source);
        }

        public abstract string Source { get; }

        public virtual Encoding ContentEncoding => Encoding.UTF8;

        public virtual string[] RemoveStrings => null;

        public virtual async Task<(bool flag, string content)> CrawlerChapterContent(string chapterLink)
        {
            var respone = await _client.GetStringAsync(chapterLink, ContentEncoding,false);
            var html = respone.html;
            string content = "";
            bool flag = true;
            if (respone.statusCode == 200)
            {
                content = AnalysisContent(html);
                if (Regex.IsMatch(content, @"^[a-zA-z]+://[^\s]*$"))
                {
                    respone = await _client.GetStringAsync(content, ContentEncoding);
                    content = AnalysisContent(respone.html);
                }
                if (!string.IsNullOrEmpty(content) && RemoveStrings != null)
                {
                    foreach (var item in RemoveStrings)
                    {
                        content = content.Replace(item, "");
                    }
                }
            }
            else if(respone.statusCode.ToString().StartsWith("4"))
            {
                flag = false;
            }
            
            return (flag,content);
        }
        protected abstract string AnalysisContent(string html);
    }
}