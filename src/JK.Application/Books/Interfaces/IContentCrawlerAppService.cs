using Abp.Dependency;
using System.Threading.Tasks;

namespace JK.Books.Interfaces
{
    public interface IContentCrawlerAppService : ITransientDependency
    {
        Task<(bool flag,string src)> CrawlContent(string authorName, string bookName, BookChapter bookChapter, bool hasSameTitle);
    }
}