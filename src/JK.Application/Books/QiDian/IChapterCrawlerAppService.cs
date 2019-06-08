using Abp.Application.Services;

namespace JK.Books.QiDian
{
    public interface IChapterCrawlerAppService : IApplicationService
    {
       bool CrawlQiDianChapter(int bookId);
    }
}