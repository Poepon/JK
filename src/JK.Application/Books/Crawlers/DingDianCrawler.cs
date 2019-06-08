using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using Abp.Dependency;
using JK.Books.Crawlers;
using JK.Books.Dto;
using HtmlAgilityPack;

namespace JK.Books
{
    public class DingDianBookCrawler : CrawlerBase, IBookCrawler
    {
        public DingDianBookCrawler(IHttpClientFactory httpClientFactory) : base(httpClientFactory)
        {
        }

        public override string SearchUrlFormat => "https://www.x23us.com/modules/article/search.php?searchtype=keywords&searchkey={0}";

        public override string Source => BookSource.DingDian;

        public override string Host => "https://www.x23us.com";
        //搜索间隔限制6秒一次
        public override int DelaySecond => 6;

        public override int SkipItems => 0;

        public override Encoding ContentEncoding => Encoding.GetEncoding("gb2312");

        public override Encoding UrlEncoding => Encoding.GetEncoding("gb2312");

        public override Encoding ChapterLinkEncoding => Encoding.GetEncoding("gb2312");

        protected override string AnalysisBookLink(string bookName, string author, string url, string html)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(html);
            if (url.Contains("search.php"))
            {
                var nodes = doc.DocumentNode.SelectNodes("//*[@id=\"content\"]/table[1]/tr");
                if (nodes != null)
                {
                    int count = 0;
                    foreach (var node in nodes)
                    {
                        if (count == 0)
                        {
                            count++;
                            continue;
                        }

                        var a = node.SelectSingleNode("td[1]/a");
                        var a2 = node.SelectSingleNode("td[2]/a");
                        var tempAuthor = node.SelectSingleNode("td[3]").InnerText.Trim();
                        var tempBookUrl = a2.GetAttributeValue("href", "");
                        var tempBookName = a.InnerText.Trim();
                        if (bookName == tempBookName && author == tempAuthor)
                        {
                            return tempBookUrl;
                        }
                    }
                }
            }
            else
            {
                var node = doc.DocumentNode.SelectSingleNode("//*[@id=\"content\"]");
                var bookLinkNode = node.SelectSingleNode( "dd[2]/div[2]/p[2]/a[1]");
                
                var tempAuthor = node.SelectSingleNode("//*[@id=\"at\"]/tr[1]/td[2]")
                    ?.InnerText.Replace("&nbsp;", "").Trim();
                if (author == tempAuthor)
                {
                    return bookLinkNode?.GetAttributeValue("href","");
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
                Name = doc.DocumentNode.SelectSingleNode("//*[@id=\"a_main\"]/div[2]/dl[1]/dd[1]/h1[1]").InnerText.Replace("最新章节更新列表", "").Trim(),
                Source = Source,
                //Introduce = doc.DocumentNode.SelectSingleNode("//*[@id=\"intro\"]").InnerText,
                //ImageSrc = doc.DocumentNode.SelectSingleNode("//*[@id=\"fmimg\"]/img[1]").GetAttributeValue("src", ""),
                TotalNumberOfWords = 0,
                //LastUpdateDateTime = DateTime.Parse(doc.DocumentNode.SelectSingleNode("//*[@id=\"info\"]/p[3]").InnerText.Replace("最后更新：", "")),
                Status = BookStatus.Serializing,
                //AuthorName = doc.DocumentNode.SelectSingleNode("//*[@id=\"info\"]/p[1]").InnerText.Replace("作&nbsp;&nbsp;&nbsp;&nbsp;者：", "")
            };
            var text = doc.DocumentNode.SelectSingleNode("//*[@id=\"a_main\"]/div[2]/dl[1]/dd[2]/h3[1]").InnerText;
            var match = Regex.Match(text, "作者：(?<author>.*?)&nbsp;&nbsp;");
            if (match.Success)
            {
                entity.AuthorName = match.Groups["author"].Value;
            }

            var chapterNodes = doc.DocumentNode.SelectNodes("//*[@id=\"at\"]//a");
            var links = new List<BookChapterLinkDto>();
            for (int i = SkipItems; i < chapterNodes.Count; i++)
            {
                var chapterNode = chapterNodes[i];
                var chapterUrl = chapterNode.GetAttributeValue("href", "");
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
    public class DingDianContentCrawler : ContentCrawlerBase, IContentCrawler
    {
        public DingDianContentCrawler(IHttpClientFactory httpClientFactory) : base(httpClientFactory)
        {
        }

        public override string Source => BookSource.DingDian;
        public override Encoding ContentEncoding => Encoding.GetEncoding("gb2312");
        public override string[] RemoveStrings => new string[] {
            "www.x23us.com",
            "顶点小说"
        };
        protected override string AnalysisContent(string html)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(html);
            var node = doc.DocumentNode.SelectSingleNode("//*[@id=\"contents\"]");
            var content = node?.InnerHtml;
            return content;
        }
    }

}