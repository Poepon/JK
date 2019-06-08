using System.Net.Http;
using System.Text;
using JK.Books.Crawlers;
using HtmlAgilityPack;

namespace JK.Books
{
    public class BaYiContentCrawler : ContentCrawlerBase, IContentCrawler
    {
        public BaYiContentCrawler(IHttpClientFactory httpClientFactory) : base(httpClientFactory)
        {
        }

        public override string Source => BookSource.BaYi;

        public override Encoding ContentEncoding => Encoding.GetEncoding("gb2312");

        public override string[] RemoveStrings => new string[] {
            "www.zwdu.com",
            "m.zwdu.com",
            "中文网"
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