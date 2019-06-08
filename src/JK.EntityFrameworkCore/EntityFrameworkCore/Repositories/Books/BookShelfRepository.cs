using Abp.Domain.Repositories;
using Abp.EntityFrameworkCore;
using JK.Books;
using JK.EntityFrameworkCore;
using JK.EntityFrameworkCore.Repositories;
using JK.IRepositories;

namespace JK.Repositories
{
    public class BookShelfRepository : JKRepositoryBase<BookShelf>, IBookShelfRepository
    {
        public BookShelfRepository(IDbContextProvider<JKDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }
    }
}
