using System.Net.Http;
using System.Text;
using JK.Books.Crawlers;
using HtmlAgilityPack;

namespace JK.Books
{
    public class BiqugeTwContentCrawler : ContentCrawlerBase, IContentCrawler
    {
        public BiqugeTwContentCrawler(IHttpClientFactory httpClientFactory) : base(httpClientFactory)
        {
            //Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }

        public override string Source => BookSource.BiQuGe_TW;

        public override Encoding ContentEncoding => Encoding.GetEncoding("gb2312");

        public override string[] RemoveStrings => new string[] {
            "www.biquge.com.tw",
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