using Abp.Application.Services;
using Abp.Dependency;
using Abp.Domain.Repositories;
using JK.IRepositories;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace JK.Books.Interfaces
{
    public interface IBookChapterContentAppService : IApplicationService, ITransientDependency
    {
        void CrawlerContent(int bookId, int? chapterId = null);
    }

    public class BookChapterContentAppService : JKAppServiceBase, IBookChapterContentAppService
    {
        private readonly IBookRepository _bookRepository;
        private readonly IBookChapterRepository _chapterRepository;
        private readonly IContentCrawlerAppService contentCrawler;

        public BookChapterContentAppService(
            IBookRepository bookRepository,
            IBookChapterRepository bookChapterRepository,
            IContentCrawlerAppService contentCrawler)
        {
            this._bookRepository = bookRepository;
            this._chapterRepository = bookChapterRepository;
            this.contentCrawler = contentCrawler;
        }
        public void CrawlerContent(int bookId, int? chapterId = null)
        {
            if (chapterId.HasValue)
            {
                var chapter = _chapterRepository.GetAll().Include(c => c.Book).ThenInclude(b => b.Author)
                .Include(c => c.ChapterLinks).SingleOrDefault(c => c.Id == chapterId.Value);
                if (chapter != null && chapter.IsActive)
                    CrawlerContent(chapter);
            }
            else
            {
                var chapters = _chapterRepository.GetAll()
               .Include(c => c.Book).ThenInclude(b => b.Author).Include(c => c.ChapterLinks)
               .Where(c => c.BookId == bookId && c.HasSrc == false && c.IsActive && c.ChapterLinks.Any()).ToList();
                foreach (var chapter in chapters)
                {
                    CrawlerContent(chapter);
                }
            }
        }

        private void CrawlerContent(BookChapter chapter)
        {
            if (chapter.HasSrc == false)
            {
                var hasSameTitle = _chapterRepository.Count(c => c.BookId == chapter.BookId && c.Title == chapter.Title) > 1;
                var respone = contentCrawler.CrawlContent(chapter.Book.Author.Name, chapter.Book.Name, chapter, hasSameTitle).Result;
                if (respone.flag)
                {
                    if (!string.IsNullOrEmpty(respone.src))
                    {
                        chapter.Src = respone.src;
                        chapter.HasSrc = true;
                        _chapterRepository.Update(chapter);
                        Logger.Info($"{chapter.BookId}-{chapter.Title}抓取成功。");
                    }
                }
                else
                {
                    chapter.IsActive = false;
                    _chapterRepository.Update(chapter);
                    Logger.Info($"{chapter.BookId}-{chapter.Title}设为无效。");
                }
            }
        }
    }
}