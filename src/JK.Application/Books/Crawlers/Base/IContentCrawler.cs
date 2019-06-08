using System.Threading.Tasks;
using Abp.Dependency;

namespace JK.Books
{
    public interface IContentCrawler : ISingletonDependency
    {
        string Source { get; }

        Task<(bool flag,string content)> CrawlerChapterContent(string chapterLink);
    }
}