using Abp.Dependency;
using JK.Books.Crawlers;
using JK.Books.Dto;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Net.Http;

namespace JK.Books
{
    /// <summary>
    /// 新笔趣阁
    /// www.bxquge.com
    /// </summary>
    public class BxQuGeBookCrawler : CrawlerBase, IBookCrawler
    {
        public BxQuGeBookCrawler(IHttpClientFactory httpClientFactory) : base(httpClientFactory)
        {
        }

        public override string SearchUrlFormat => "http://www.bxquge.com/index.php?r=book/search";

        public override string SearchMethod => "POST";

        public override string SearchPostKey => "key";

        public override string Source => BookSource.Bxquge;

        public override string Host => "http://www.bxquge.com";
        //搜索间隔不限
        public override int DelaySecond => 2;

        public override int SkipItems => 0;

        protected override string AnalysisBookLink(string bookName, string author, string url, string html)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(html);
            if (url.Contains("search"))
            {
                var nodes = doc.DocumentNode.SelectNodes("//*[@id=\"hotcontent\"]/div[1]/div");
                if (nodes != null && nodes.Count > 1)
                {
                    for (int i = 1; i < nodes.Count; i++)
                    {
                        var node = nodes[i];
                        var a = node.SelectSingleNode("dl[1]/dt[1]/a[1]");
                        var tempAuthor = node.SelectSingleNode("dl[1]/dt[1]/span[1]").InnerText;
                        var tempBookUrl = a.GetAttributeValue("href", "");
                        tempBookUrl = tempBookUrl.Contains(Host) ? tempBookUrl : Host + tempBookUrl;
                        var tempBookName = a.InnerText;
                        if (bookName == tempBookName && author == tempAuthor)
                        {
                            return tempBookUrl;
                        }
                    }
                }

                return "";
            }
            else
            {
                var tempAuthor = doc.DocumentNode.SelectSingleNode("//*[@id=\"info\"]/p[1]")?.InnerText.Replace("作&nbsp;&nbsp;&nbsp;&nbsp;者：", "");
                if (author == tempAuthor)
                {
                    {
                        return url;
                    }
                }
                return "";
            }
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
                Status = BookStatus.Serializing,//
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
                    Link = bookLink + chapterUrl,
                    Source = Source
                };
                links.Add(link);
            }

            return new ValueTuple<CreateBookDto, List<BookChapterLinkDto>>(entity, links);
        }
    }
}
