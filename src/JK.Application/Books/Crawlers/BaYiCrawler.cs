using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using JK.Books.Crawlers;
using JK.Books.Dto;
using HtmlAgilityPack;

namespace JK.Books
{
    public class BaYiBookCrawler : CrawlerBase, IBookCrawler
    {
        public BaYiBookCrawler(IHttpClientFactory httpClientFactory) : base(httpClientFactory)
        {
        }

        public override string SearchUrlFormat => "https://www.zwdu.com/search.php?keyword={0}";

        public override string Source => BookSource.BaYi;

        public override string Host => "https://www.zwdu.com";
        
        ///搜索不限间隔
        public override int DelaySecond => 2;

        public override int SkipItems => 0;

        public override Encoding ContentEncoding => Encoding.UTF8;

        public override Encoding UrlEncoding => Encoding.UTF8;

        public override Encoding ChapterLinkEncoding => Encoding.GetEncoding("gb2312");


        protected override string AnalysisBookLink(string bookName, string author, string url, string html)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(html);
            var nodes = doc.DocumentNode.SelectNodes("/html[1]/body[1]/div[3]/div");
            if (nodes != null)
            {
                foreach (var node in nodes)
                {
                    var a = node.SelectSingleNode("div[2]/h3[1]/a");
                    var tempAuthor = node.SelectSingleNode("div[2]/div[1]/p[1]/span[2]").InnerText.Trim();
                    var tempBookUrl = a.GetAttributeValue("href", "");
                    var tempBookName = a.InnerText.Trim();
                    if (bookName == tempBookName && author == tempAuthor)
                    {
                        return tempBookUrl;
                    }
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