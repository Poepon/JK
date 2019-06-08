using HtmlAgilityPack;
using System.Net.Http;

namespace JK.Books.Crawlers
{
    public class BxQuGeContentCrawler : ContentCrawlerBase, IContentCrawler
    {
        public BxQuGeContentCrawler(IHttpClientFactory httpClientFactory) : base(httpClientFactory)
        {
        }

        public override string Source => BookSource.Bxquge;

        public override string[] RemoveStrings => new string[] {
            "www.bxquge.com",
        };

        protected override string AnalysisContent(string html)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(html);
            var node = doc.DocumentNode.SelectSingleNode("//*[@id=\"content\"]");
            var content = node?.InnerHtml;
            return content;
        }
    }
}
