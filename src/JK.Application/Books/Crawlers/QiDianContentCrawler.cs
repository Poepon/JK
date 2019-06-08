using System.Net.Http;
using System.Text.RegularExpressions;
using HtmlAgilityPack;

namespace JK.Books.Crawlers
{
    public class QiDianContentCrawler :ContentCrawlerBase, IContentCrawler
    {
        public override string Source => BookSource.QiDian;
        protected override string AnalysisContent(string html)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(html);
            string content = string.Empty;
            var scriptNode = doc.DocumentNode.SelectSingleNode("/html[1]/body[1]/script[17]");
            if (scriptNode != null)
            {
                var m = Regex.Match(scriptNode.InnerHtml, "\"content\":\"(.*?)\",\"authorWords\"");
                if (m.Success)
                {
                    content = m.Value.Replace("\"content\":\"", "").Replace("\",\"authorWords\"", "");
                }
            }
            if (string.IsNullOrEmpty(content))
            {
                var node = doc.DocumentNode.SelectSingleNode("/html[1]/body[1]/div[2]/content[1]/article[1]/section[1]");
                content = node?.InnerHtml;
            }

            return content;
        }


        public QiDianContentCrawler(IHttpClientFactory httpClientFactory) : base(httpClientFactory)
        {
        }
    }
}