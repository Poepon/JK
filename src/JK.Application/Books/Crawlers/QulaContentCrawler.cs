using System.Net.Http;
using System.Text.RegularExpressions;
using HtmlAgilityPack;

namespace JK.Books.Crawlers
{
    public class QulaContentCrawler : ContentCrawlerBase, IContentCrawler
    {
        public QulaContentCrawler(IHttpClientFactory httpClientFactory) : base(httpClientFactory)
        {
        }

        public override string Source => BookSource.Qula;

        public override string[] RemoveStrings => new string[] {
            "www.qu.la",
            "<script>chaptererror();</script>",
            "<div class=\"adread\"><script>();</script>"
        };

        protected override string AnalysisContent(string html)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(html);
            var node = doc.DocumentNode.SelectSingleNode("//*[@id=\"content\"]");
            if (node == null)
            {
                var text = doc.DocumentNode.SelectSingleNode("/html[1]/head[1]/script[1]").InnerHtml;
                Regex regex = new Regex(@"[a-zA-z]+://[^\s]*");
                var m = regex.Match(text);
                if (m.Success)
                {
                    var url = m.Value.TrimEnd("';".ToCharArray());
                    return url;
                }
            }
            else
            {
                return node.InnerHtml;
            }
            return "";
        }
    }
}