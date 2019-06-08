using Abp.EntityFrameworkCore;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System;
using JK.Books;

namespace JK.EntityFrameworkCore.Repositories
{
    public class BookRepository : JKRepositoryBase<Book>, IBookRepository
    {
        public BookRepository(IDbContextProvider<JKDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }

        public IQueryable<Book> GetHasUpdateBookList()
        {
            bool UseMySql = false;
            if (UseMySql)
            {
                string sql = "select * from Books where LastUpdateDateTime>(select ifnull(max(UpdateTime),'2000-1-1')  from BookChapters where BookId=Books.Id)";
                var rs = this.Context.Books.FromSql(sql);
                return rs;
            }
            else
            {
                string sql = "select * from Books where LastUpdateDateTime>(select isnull(max(UpdateTime),'2000-1-1')  from BookChapters where BookId=Books.Id)";
                var rs = this.Context.Books.FromSql(sql);
                return rs;
            }
        }

        public DateTime? GetLastUpdateTime(int bookId)
        {
            if (Context.BookChapters.Any(c => c.BookId == bookId))
            {
                return Context.BookChapters.Where(c => c.BookId == bookId).Max(c => c.UpdateTime);
            }
            else
            {
                return null;
            }
        }
    }
}
