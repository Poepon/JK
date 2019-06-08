using Abp.Domain.Repositories;
using Abp.EntityFrameworkCore;
using JK.Books;
using JK.EntityFrameworkCore;
using JK.EntityFrameworkCore.Repositories;
using JK.IRepositories;

namespace JK.Repositories
{
    public class SourceIgnoreBookRepository : JKRepositoryBase<NotFoundBook>, ISourceIgnoreBookRepository
    {
        public SourceIgnoreBookRepository(IDbContextProvider<JKDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }
    }
}
