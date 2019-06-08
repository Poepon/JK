using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using Abp.BackgroundJobs;
using Abp.Dependency;
using Abp.Threading.Timers;
using JK.Books.Crawlers;
using JK.Books.Dto;
using JK.Books.Interfaces;
using JK.Books.Jobs;
using HtmlAgilityPack;

namespace JK.Books.Workers.QiDian
{
    /// <summary>
    /// 起点图书爬虫
    /// </summary>
    public abstract class QiDianBookCrawlerWorker : PoeponPeriodicBackgroundWorkerBase, ISingletonDependency
    {
        protected virtual WebSite Web { get; set; } = WebSite.Boy;
        protected virtual BookStatus? 完本状态 { get; set; } = BookStatus.Serializing;
        protected virtual 更新时间 最后更新 { get; set; } = 更新时间.全部;
        protected virtual 显示方式 显示 { get; set; } = 显示方式.栅格方式;
        protected virtual 排序方式 排序 { get; set; } = 排序方式.总字数;

        protected virtual bool 只看三个月内新书 { get; set; } = false;

        protected virtual bool CreateOrUpdate { get; set; } = false;

        private readonly IBookAppService bookAppService;
        private readonly IBookLinkAppService bookLinkAppService;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IBackgroundJobManager _backgroundJobManager;

        public string GetUrl()
        {
            var sb = new StringBuilder();
            if (Web == WebSite.Boy)
            {
                sb.Append("https://www.qidian.com/all?");
            }
            else
            {
                sb.Append("https://www.qidian.com/mm/all?");
            }
            if (排序 != 排序方式.人气排序)
            {
                sb.Append("orderId=" + Convert.ToInt32(排序) + "&");
            }
            if (完本状态 == BookStatus.Serializing)
            {
                sb.Append("action=0&");
            }
            else if (完本状态 == BookStatus.BookEnd)
            {
                sb.Append("action=1&");
            }
            if (最后更新 != 更新时间.全部)
            {
                sb.Append("update=" + Convert.ToInt32(最后更新) + "&");
            }
            if (只看三个月内新书)
            {
                sb.Append("month=3&");
            }
            sb.Append("style=" + Convert.ToInt32(显示) + "&");
            if (显示 == 显示方式.列表方式)
            {
                sb.Append("pageSize=50&");
            }
            else
            {
                sb.Append("pageSize=20&");
            }
            if (Web == WebSite.Boy)
            {
                sb.Append("siteid=1&");
            }
            else if (Web == WebSite.Girl)
            {
                sb.Append("siteid=0&");
            }
            sb.Append("vip=1&sign=1&pubflag=0&hiddenField=0&page={0}");
            return sb.ToString();
        }

        public QiDianBookCrawlerWorker(AbpTimer timer,
            IBookAppService bookAppService,
            IBookLinkAppService bookLinkAppService,
            IHttpClientFactory httpClientFactory,
            IBackgroundJobManager backgroundJobManager)
            : base(timer)
        {
            this.bookAppService = bookAppService;
            this.bookLinkAppService = bookLinkAppService;
            _httpClientFactory = httpClientFactory;
            _backgroundJobManager = backgroundJobManager;
            Timer.Period = CheckPeriodAsMilliseconds;
            Timer.RunOnStart = true;
        }

        public virtual int CheckPeriodAsMilliseconds { get; } = 1 * 60 * 60 * 1000; //1 小时

        protected override void DoSomething()
        {
            int pageIndex = 1;
            int skipCount = 0;
            var pageCount = 0;
            var pageSize = 显示 == 显示方式.栅格方式 ? 20 : 50;
            var _httpClient = _httpClientFactory.CreateClient(BookSource.QiDian);
            Logger.Info($"爬起点{(完本状态.HasValue ? 完本状态.ToString() : "ALL")}图书开始。");
            try
            {
                if (完本状态.HasValue)
                {
                    if (排序 == 排序方式.更新时间 || 排序 == 排序方式.完本时间)
                    {
                        int days = 3;
                        switch (最后更新)
                        {
                            case 更新时间.三日内:
                                days = 3;
                                break;
                            case 更新时间.七日内:
                                days = 7;
                                break;
                            case 更新时间.半月内:
                                days = 15;
                                break;
                            case 更新时间.一月内:
                                days = 30;
                                break;
                            case 更新时间.全部:
                                days = 3;
                                break;
                        }
                        DateTime today = DateTime.Today.AddDays(-days);
                        skipCount = bookAppService.Count(b => b.LastUpdateDateTime > today && b.Status == 完本状态);
                    }
                    else
                    {
                        int count = bookAppService.Count(b => b.Status == 完本状态);
                        pageIndex = count / pageSize;
                    }
                }

                var urlFormat = GetUrl();
                do
                {
                    var url = string.Format(urlFormat, pageIndex);
                    var html = string.Empty;
                    try
                    {
                        html = _httpClient.GetString(url, Encoding.UTF8).html;
                    }
                    catch (Exception e)
                    {
                        Logger.Error(e.Message);
                        Thread.Sleep(2000);
                        continue;
                    }

                    var doc = new HtmlDocument();
                    doc.LoadHtml(html);
                    if (pageCount == 0)
                    {
                        var node = doc.DocumentNode.SelectSingleNode("/html[1]/body[1]/div[2]/div[5]/div[2]/div[1]/div[2]/div[3]/div[1]/span[1]");
                        pageCount = node != null ? Convert.ToInt32(Math.Ceiling((decimal.Parse(node.InnerText) - skipCount) / pageSize)) : 40000;
                        Logger.Info($"{(完本状态.HasValue ? 完本状态.ToString() : "")}页数{pageCount}。");
                    }
                    var list = 显示 == 显示方式.列表方式 ? ProcessList(doc) : ProcessGrid(doc);
                    if (list.Count == 0)
                    {
                        Logger.Info($"内容为空，退出。{(完本状态.HasValue ? 完本状态.ToString() : "")}第{pageIndex}页。");
                        return;
                    }

                    foreach (var book in list)
                    {
                        if (CreateOrUpdate)
                        {
                            if (!bookAppService.Any(b => b.SourceBookId == book.SourceBookId))
                            {
                                var dto = bookAppService.Create(book);
                                //_backgroundJobManager.Enqueue<BookCoverCrawlJob, BookCoverCrawlJobArgs>
                                //    (new BookCoverCrawlJobArgs() { BookId = dto.Id });
                                var bookLink = new BookLinkDto()
                                {
                                    BookId = dto.Id,
                                    Source = BookSource.QiDian,
                                    Link = $"https://m.qidian.com/book/{book.SourceBookId}/catalog",
                                    LastAccessTime = new DateTime(2000, 1, 1)
                                };

                                bookLinkAppService.Create(bookLink);
                            }
                        }
                        else
                        {
                            if (bookAppService.Any(b => b.SourceBookId == book.SourceBookId))
                            {
                                var dto = bookAppService.Get(b => b.SourceBookId == book.SourceBookId);
                                bool hasChange = false;
                                if (dto.Name != book.Name)
                                {
                                    dto.Name = book.Name;
                                    hasChange = true;
                                }
                                if (dto.Status == BookStatus.Serializing && 完本状态 == BookStatus.BookEnd)
                                {
                                    dto.Status = BookStatus.BookEnd;
                                    hasChange = true;
                                }
                                if (book.LastUpdateDateTime > dto.LastUpdateDateTime)
                                {
                                    dto.LastUpdateDateTime = new DateTime(book.LastUpdateDateTime.Year,
                                        book.LastUpdateDateTime.Month, book.LastUpdateDateTime.Day,
                                        book.LastUpdateDateTime.Hour, book.LastUpdateDateTime.Minute, 0);
                                    if (dto.LastUpdateDateTime > dto.LastCrawlDateTime)
                                    {
                                        //_backgroundJobManager.Enqueue<QiDianChapterCrawlJob, QiDianChapterCrawlJobArgs>(
                                        //    new QiDianChapterCrawlJobArgs()
                                        //    {
                                        //        BookId = dto.Id
                                        //    }, delay: TimeSpan.FromMinutes(1));
                                    }
                                    hasChange = true;
                                }
                                if (book.TotalNumberOfWords > dto.TotalNumberOfWords)
                                {
                                    dto.TotalNumberOfWords = book.TotalNumberOfWords;
                                    hasChange = true;
                                }
                                if (hasChange)
                                {
                                    bookAppService.Update(dto);
                                }
                            }
                        }
                    }
                    Logger.Info($"{(完本状态.HasValue ? 完本状态.ToString() : "")}第{pageIndex}页抓取完成。");
                    pageIndex++;
                    Thread.Sleep(2000);
                } while (pageIndex <= pageCount);
            }
            finally
            {
                Logger.Info($"爬起点{(完本状态.HasValue ? 完本状态.ToString() : "")}图书结束。");
            }
        }

        private List<CreateBookDto> ProcessGrid(HtmlDocument doc)
        {
            var result = new List<CreateBookDto>();
            string xpath = "/html/body/div[2]/div[5]/div[2]/div[2]/div/ul";
            var mainNode = doc.DocumentNode.SelectSingleNode(xpath);
            if (mainNode == null)
            {
                return result;
            }
            var bookNodes = mainNode.SelectNodes("li");
            if (bookNodes != null)
            {
                foreach (var bookNode in bookNodes)
                {
                    var booknamenode = bookNode.SelectSingleNode("div[2]/h4/a");
                    var link = booknamenode.GetAttributeValue("href", "");
                    var cntStr = bookNode.SelectSingleNode("div[2]/p[3]/span").InnerText;
                    decimal cnt;
                    if (cntStr.Contains("万"))
                    {
                        var d = decimal.TryParse(cntStr.Replace("万", "").Replace("字", ""), out cnt);
                        if (!d)
                        {
                            cnt = 0;
                        }
                    }
                    else
                    {
                        cnt = 0;
                    }

                    var book = new CreateBookDto
                    {
                        Source = BookSource.QiDian,
                        Name = booknamenode.InnerText,
                        Introduce = bookNode.SelectSingleNode("div[2]/p[2]").InnerText.Trim(),
                        ImageSrc = "https:" + bookNode.SelectSingleNode("div[1]/a/img").GetAttributeValue("src", ""),
                        TotalNumberOfWords = cnt,
                        LastUpdateDateTime = new DateTime(2000, 1, 1),
                        IsBest = false,
                    };
                    var statusString = bookNode.SelectSingleNode("div[2]/p[1]/span[1]").InnerText.Trim();
                    book.Status = statusString == "连载中" ? BookStatus.Serializing : BookStatus.BookEnd;
                    book.SourceBookId = booknamenode.GetAttributeValue("data-bid", 0);
                    book.AuthorName = bookNode.SelectSingleNode("div[2]/p[1]/a[1]").InnerText;
                    book.BookTypeName = bookNode.SelectSingleNode("div[2]/p[1]/a[2]").InnerText;
                    result.Add(book);
                }
            }
            return result;
        }

        private List<CreateBookDto> ProcessList(HtmlDocument doc)
        {
            var result = new List<CreateBookDto>();
            var xpath = "/html[1]/body[1]/div[2]/div[5]/div[2]/div[2]/div[1]/table[1]/tbody[1]";
            var mainNode = doc.DocumentNode.SelectSingleNode(xpath);
            if (mainNode == null)
            {
                return result;
            }
            var bookNodes = mainNode.SelectNodes("tr");
            if (bookNodes != null && bookNodes.Count > 1)
            {
                for (int i = 0; i < bookNodes.Count; i++)
                {
                    var bookNode = bookNodes[i];
                    var booknamenode = bookNode.SelectSingleNode("td[2]/a");
                    var link = booknamenode.GetAttributeValue("href", "");
                    var dateStr = bookNode.SelectSingleNode("td[6]").InnerText;
                    DateTime ut;
                    if (dateStr.Contains("小时前"))
                    {
                        var now = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, 0, 0);
                        ut = now.AddHours(-int.Parse(dateStr.Replace("小时前", "")));
                    }
                    else if (dateStr.Contains("分钟前"))
                    {
                        var now = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, 0);
                        ut = now.AddMinutes(-int.Parse(dateStr.Replace("分钟前", "")));
                    }
                    else
                    {
                        var bl = DateTime.TryParse(dateStr
                          .Replace("今天", DateTime.Today.ToString("yyyy-MM-dd "))
                          .Replace("昨日", DateTime.Today.AddDays(-1).ToString("yyyy-MM-dd "))
                          .Replace("昨天", DateTime.Today.AddDays(-1).ToString("yyyy-MM-dd "))
                          .Replace("更新", "")
                          , out ut);
                        if (!bl)
                        {
                            Logger.Error("更新时间异常：" + dateStr);
                            ut = new DateTime(2000, 1, 1);
                        }
                        ut = ut.AddSeconds(-ut.Second);
                    }
                    var cntStr = bookNode.SelectSingleNode("td[4]/span").InnerText;
                    decimal cnt;
                    if (cntStr.Contains("万"))
                    {
                        var d = decimal.TryParse(cntStr.Replace("万", "").Replace("字", ""), out cnt);
                        if (!d)
                        {
                            cnt = 0;
                        }
                    }
                    else
                    {
                        cnt = 0;
                    }

                    var book = new CreateBookDto
                    {
                        Source = BookSource.QiDian,
                        Name = booknamenode.InnerText,
                        //Introduce = bookNode.SelectSingleNode("div[2]/p[2]").InnerText.Trim(),
                        //ImageSrc = "https:" + bookNode.SelectSingleNode("div[1]/a/img").GetAttributeValue("src", ""),
                        TotalNumberOfWords = cnt,
                        LastUpdateDateTime = ut,
                        Status = 完本状态.GetValueOrDefault(BookStatus.Serializing),
                        IsBest = false
                    };
                    book.SourceBookId = booknamenode.GetAttributeValue("data-bid", 0);
                    book.AuthorName = bookNode.SelectSingleNode("td[5]/a[1]").InnerText;
                    book.BookTypeName = bookNode.SelectSingleNode("td[1]/a[1]").InnerText.Replace("「", "");

                    result.Add(book);
                }
            }
            return result;
        }

        #region 枚举

        public enum WebSite
        {
            Boy = 1,
            Girl = 2
        }

        public enum 小说品质
        {
            签约 = 1,
            精品 = 2
        }
        public enum 更新时间
        {
            全部 = 0,
            三日内 = 1,
            七日内 = 2,
            半月内 = 3,
            一月内 = 4
        }
        public enum 显示方式
        {
            栅格方式 = 1,
            列表方式 = 2
        }
        public enum 排序方式
        {
            人气排序 = 0,
            更新时间 = 5,
            总收藏 = 11,
            总字数 = 3,
            完本时间 = 6
        }

        #endregion

    }
}
