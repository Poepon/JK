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
    public class QulaBookCrawler : CrawlerBase, IBookCrawler
    {
        public QulaBookCrawler(IHttpClientFactory httpClientFactory) : base(httpClientFactory)
        {
        }

        public override string SearchUrlFormat => "https://sou.xanbhx.com/search?siteid=qula&q={0}";

        public override string Source => BookSource.Qula;

        public override string Host => "https://www.qu.la";
        //搜索间隔不限
        public override int DelaySecond => 2;

        public override Encoding ContentEncoding => Encoding.UTF8;

        public override int SkipItems => 12;

        protected override (CreateBookDto, List<BookChapterLinkDto>) AnalysisChapterLinks(string bookLink,string html)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(html);
            if (!DateTime.TryParse(
                doc.DocumentNode.SelectSingleNode("/html[1]/body[1]/div[1]/div[5]/div[2]/div[1]/p[3]").InnerText,
                out DateTime lastUpdateTime))
            {
                lastUpdateTime = new DateTime(2000, 1, 1);
            }

            var entity = new CreateBookDto()
            {
                Name = doc.DocumentNode.SelectSingleNode("/html[1]/body[1]/div[1]/div[5]/div[2]/div[1]/h1[1]").InnerText,
                Source = Source,
                Introduce = doc.DocumentNode.SelectSingleNode("/html[1]/body[1]/div[1]/div[5]/div[2]/div[2]").InnerText,
                ImageSrc = doc.DocumentNode.SelectSingleNode("/html[1]/body[1]/div[1]/div[5]/div[3]/div[1]/img")
                    .GetAttributeValue("src", ""),
                TotalNumberOfWords = 0,
                LastUpdateDateTime = lastUpdateTime.AddSeconds(-lastUpdateTime.Second),
                Status = BookStatus.Serializing,
                AuthorName = doc.DocumentNode.SelectSingleNode("/html[1]/body[1]/div[1]/div[5]/div[2]/div[1]/p[1]").InnerText
                    .Replace("作&nbsp;&nbsp;者：", "")
            };
            var chapterNodes = doc.DocumentNode.SelectNodes("/html[1]/body[1]/div[1]/div[6]/div[1]/dl[1]/dd");
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

        protected override string AnalysisBookLink(string bookName, string author, string url, string html)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(html);
            var nodes = doc.DocumentNode.SelectNodes("//*[@id=\"search-main\"]/div[1]/ul[1]/li");
            if (nodes != null && nodes.Count > 1)
            {
                for (int i = 1; i < nodes.Count; i++)
                {
                    var node = nodes[i];
                    var tdNodes = node.SelectNodes("span");
                    var a = tdNodes[1].SelectSingleNode("a");
                    var tempAuthor = tdNodes[3].InnerText;
                    var tempBookUrl = a.GetAttributeValue("href", "");
                    tempBookUrl = tempBookUrl.Contains(Host) ? tempBookUrl : Host + tempBookUrl;
                    var tempBookName = a.InnerText.Trim();
                    if (bookName == tempBookName && author == tempAuthor)
                    {
                        {
                            return tempBookUrl;
                        }
                    }
                }
            }

            return "";
        }
      
    }
}