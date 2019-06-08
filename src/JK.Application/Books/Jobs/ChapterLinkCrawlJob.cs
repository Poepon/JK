using Abp.Application.Services.Dto;
using Abp.BackgroundJobs;
using Abp.Dependency;
using Abp.Threading;
using JK.Books.Dto;
using JK.Books.Interfaces;
using JK.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JK.Books.Jobs
{
    [Serializable]
    public class ChapterLinkCrawlJobArgs
    {
        public int BookId { get; set; }

        public int BookLinkId { get; set; }

    }
    public class ChapterLinkCrawlJob : BackgroundJob<ChapterLinkCrawlJobArgs>, ITransientDependency
    {
        private readonly WorkerConfig _workerConfig;
        private readonly IBookAppService bookAppService;
        private readonly IBookLinkAppService bookLinkAppService;
        private readonly IBookChapterAppService bookChapterAppService;
        private readonly IBookChapterLinkAppService chapterLinkAppService;
        private readonly IBackgroundJobManager backgroundJobManager;


        public ChapterLinkCrawlJob(
            IBookAppService bookAppService,
            IBookLinkAppService bookLinkAppService,
            IBookChapterAppService bookChapterAppService,
            IBookChapterLinkAppService chapterLinkAppService,
            IBackgroundJobManager backgroundJobManager,
            IOptions<WorkerConfig> options)
        {
            this.bookAppService = bookAppService;
            this.bookLinkAppService = bookLinkAppService;
            this.bookChapterAppService = bookChapterAppService;
            this.chapterLinkAppService = chapterLinkAppService;
            this.backgroundJobManager = backgroundJobManager;
            _workerConfig = options.Value;
        }
        public override void Execute(ChapterLinkCrawlJobArgs args)
        {
            var bookLink = bookLinkAppService.Get(args.BookLinkId);
            if (bookLink == null)
            {
                return;
            }
            if (!bookChapterAppService.Any(c => c.BookId == args.BookId))
            {
                return;
            }
            var crawler = IocManager.Instance.ResolveAll<IBookCrawler>().First(c => c.Source == bookLink.Source);
            var book = bookAppService.Get(new EntityDto(args.BookId));
            ValueTuple<CreateBookDto, List<BookChapterLinkDto>> crawlResult;
            try
            {
                crawlResult = AsyncHelper.RunSync(() => crawler.CrawlerChapterLinks(bookLink.Link));
                if (crawlResult.Item1 == null || crawlResult.Item2 == null)
                {
                    Logger.Warn($"{args.BookId}-{book.Name}-{bookLink.Source}-{bookLink.Link}解析失败。");
                    return;
                }

                if (crawlResult.Item1.Name != book.Name && crawlResult.Item1.Name != book.AliasName)
                {
                    Logger.Warn($"{args.BookId}-{book.Name}-{bookLink.Source}-{bookLink.Link}链接错误。");
                    return;
                }
            }
            catch (Exception e)
            {
                Logger.Warn(bookLink.Source + "异常-" + bookLink, e);
                return;
            }

            if (book != null && book.Name == crawlResult.Item1.Name && book.Author.Name == crawlResult.Item1.AuthorName)
            {
                foreach (var link in crawlResult.Item2)
                {
                    var chapter = bookChapterAppService.Get(c => c.BookId == book.Id && c.Title.StartsWith(link.BookChapterTitle.Substring(0, Convert.ToInt32(link.BookChapterTitle.Length * 0.7))));
                    if (chapter != null)
                    {
                        if (!chapter.IsFree)
                        {
                            link.BookChapterId = chapter.Id;
                            if (!chapterLinkAppService.Any(l => l.BookChapterId == chapter.Id
                                                                && l.Source == crawler.Source))
                            {
                                chapterLinkAppService.Create(link);
                                if (!chapter.HasSrc)
                                {
                                    if (_workerConfig.ChapterContentWorkerEnable)
                                    {
                                        //backgroundJobManager.Enqueue<ChapterContentCrawlJob, ChapterContentCrawlJobArgs>(new ChapterContentCrawlJobArgs
                                        //{
                                        //    BookId = chapter.BookId,
                                        //    ChapterId = chapter.Id
                                        //}, BackgroundJobPriority.High);
                                    }

                                }
                            }
                        }
                    }
                    else
                    {
                        //if (!chapterUnknownLinkAppService.Any(l => l.BookId == bookEntity.Id && l.Title == link.BookChapterTitle))
                        //{
                        //    chapterUnknownLinkAppService.Create(new BookChapterUnknownLinkDto
                        //    {
                        //        BookId = bookEntity.Id,
                        //        Title = link.BookChapterTitle,
                        //        Source = link.Source,
                        //        Link = link.Link
                        //    });
                        //}
                    }
                }
            }
            bookLink.LastAccessTime = DateTime.Now;
            bookLinkAppService.Update(bookLink);
            Logger.Info($"采集BookId:{bookLink.BookId}[{bookLink.Source}]成功。");
        }
    }
}