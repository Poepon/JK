using System;
using System.Collections.Generic;
using System.Text;
using Abp.Domain.Repositories;
using JK.Books;

namespace JK.IRepositories
{
    public interface IBookChapterRepository: IRepository<BookChapter>
    {
    }
}
