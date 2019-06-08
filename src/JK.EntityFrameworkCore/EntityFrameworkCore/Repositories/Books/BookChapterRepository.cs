using System;
using System.Collections.Generic;
using System.Text;
using Abp.Domain.Repositories;
using Abp.EntityFrameworkCore;
using JK.Books;
using JK.EntityFrameworkCore;
using JK.EntityFrameworkCore.Repositories;
using JK.IRepositories;

namespace JK.Repositories
{
    public class BookChapterRepository : JKRepositoryBase<BookChapter>, IBookChapterRepository
    {
        public BookChapterRepository(IDbContextProvider<JKDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }
    }
}
