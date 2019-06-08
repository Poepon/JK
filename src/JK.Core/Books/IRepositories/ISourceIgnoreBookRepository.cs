using Abp.Domain.Repositories;
using JK.Books;

namespace JK.IRepositories
{
    public interface ISourceIgnoreBookRepository : IRepository<NotFoundBook>
    {
    }
    
}
