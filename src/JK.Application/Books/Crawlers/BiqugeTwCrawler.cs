using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Abp.Dependency;
using JK.Books.Crawlers;
using JK.Books.Dto;
using HtmlAgilityPack;

namespace JK.Books
{
    public class BiqugeTwBookCrawler : CrawlerBase, IBookCrawler
    {
        public BiqugeTwBookCrawler(IHttpClientFactory httpClientFactory) : base(httpClientFactory)
        {
            //Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }

        public override string SearchUrlFormat => "http://www.biquge.com.tw/modules/article/soshu.php?searchkey={0}";

        public override Encoding ContentEncoding => Encoding.GetEncoding("gb2312");

        public override string Source => BookSource.BiQuGe_TW;

        public override string Host => "http://www.biquge.com.tw";
        
        //搜索限制间隔30秒
        public override int DelaySecond => 30;

        public override int SkipItems => 0;

        protected override string AnalysisBookLink(string bookName, string author, string url, string html)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(html);
            if (url.Contains("soshu.php"))
            {
                var nodes = doc.DocumentNode.SelectNodes("//*[@id=\"content\"]/table[1]/tr");
                if (nodes != null && nodes.Count > 1)
                {
                    for (int i = 1; i < nodes.Count; i++)
                    {
                        var node = nodes[i];
                        var tdNodes = node.SelectNodes("td");
                        var a = tdNodes[0].SelectSingleNode("a");
                        var tempAuthor = tdNodes[2].InnerText;
                        var tempBookUrl = a.GetAttributeValue("href", "");
                        tempBookUrl = tempBookUrl.Contains(Host) ? tempBookUrl : Host + tempBookUrl;
                        var tempBookName = a.InnerText;
                        if (bookName == tempBookName && author == tempAuthor)
                        {
                            return tempBookUrl;
                        }
                    }
                }
            }
            else
            {
                var tempAuthor = doc.DocumentNode.SelectSingleNode("//*[@id=\"info\"]/p[1]")?.InnerText.Replace("作&nbsp;&nbsp;&nbsp;&nbsp;者：", "");
                if (author == tempAuthor)
                {
                    return url;
                }
            }
            return "";
        }

        protected override (CreateBookDto, List<BookChapterLinkDto>) AnalysisChapterLinks(string bookLink, string html)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(html);
            var entity = new CreateBookDto()
            {
                Name = doc.DocumentNode.SelectSingleNode("//*[@id=\"info\"]/h1[1]").InnerText,
                Source = Source,
                Introduce = doc.DocumentNode.SelectSingleNode("//*[@id=\"intro\"]").InnerText,
                ImageSrc = doc.DocumentNode.SelectSingleNode("//*[@id=\"fmimg\"]/img[1]").GetAttributeValue("src", ""),
                TotalNumberOfWords = 0,
                LastUpdateDateTime = DateTime.Parse(doc.DocumentNode.SelectSingleNode("//*[@id=\"info\"]/p[3]").InnerText.Replace("最后更新：", "")),
                Status = BookStatus.Serializing,
                AuthorName = doc.DocumentNode.SelectSingleNode("//*[@id=\"info\"]/p[1]").InnerText.Replace("作&nbsp;&nbsp;&nbsp;&nbsp;者：", "")
            };
            var chapterNodes = doc.DocumentNode.SelectNodes("//*[@id=\"list\"]/dl/dd");
            var links = new List<BookChapterLinkDto>();
            for (int i = SkipItems; i < chapterNodes.Count; i++)
            {
                var chapterNode = chapterNodes[i];
                var chapterUrl = chapterNode.SelectSingleNode("a").GetAttributeValue("href", "");
                var link = new BookChapterLinkDto
                {
                    BookChapterTitle = BookChapterHtmlHelper.GetChineseTitle(chapterNode.InnerText.Trim()),
                    Link = Host + chapterUrl,
                    Source = Source
                };
                links.Add(link);
            }

            return new ValueTuple<CreateBookDto, List<BookChapterLinkDto>>(entity, links);
        }


    }
}