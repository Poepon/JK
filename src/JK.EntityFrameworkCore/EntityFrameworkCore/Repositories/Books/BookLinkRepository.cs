using Abp.Domain.Repositories;
using Abp.EntityFrameworkCore;
using JK.Books;
using JK.EntityFrameworkCore;
using JK.EntityFrameworkCore.Repositories;
using JK.IRepositories;

namespace JK.Repositories
{
    public class BookLinkRepository : JKRepositoryBase<BookLink>, IBookLinkRepository
    {
        public BookLinkRepository(IDbContextProvider<JKDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }
    }
}
