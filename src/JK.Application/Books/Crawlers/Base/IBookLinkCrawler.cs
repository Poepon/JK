using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Dependency;
using JK.Books.Dto;

namespace JK.Books
{
    public interface IBookCrawler : ISingletonDependency
    {
        string Source { get; }

        int DelaySecond { get; }

        Task<string> CrawlerBookLinks(string bookName, string author);

        Task<(CreateBookDto, List<BookChapterLinkDto>)> CrawlerChapterLinks(string bookLink);
    }
}
