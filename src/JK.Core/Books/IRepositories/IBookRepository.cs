using System;
using System.Linq;
using Abp.Application.Services;
using Abp.Domain.Repositories;

namespace JK.Books
{
    public interface IBookRepository : IRepository<Book>
    {
        IQueryable<Book> GetHasUpdateBookList();

        DateTime? GetLastUpdateTime(int bookId);

        //void SetLastUpdateTime(int bookId);

    }
}
