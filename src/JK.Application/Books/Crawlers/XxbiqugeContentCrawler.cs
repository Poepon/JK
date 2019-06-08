using System.Net.Http;
using Abp.Dependency;
using JK.Books.Crawlers;
using HtmlAgilityPack;

namespace JK.Books
{
    public class XxbiqugeContentCrawler : ContentCrawlerBase, IContentCrawler, ITransientDependency
    {
        public XxbiqugeContentCrawler(IHttpClientFactory httpClientFactory) : base(httpClientFactory)
        {
        }

        public override string Source => BookSource.Xxbiquge;

        public override string[] RemoveStrings => new string[] {
            "www.xxbiquge.com",
            "新笔趣阁"
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