using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Dependency;
using JK.Books.Dto;
using System;
using System.Linq.Expressions;

namespace JK.Books.Interfaces
{
    public interface IBookChapterLinkAppService : IApplicationService, ITransientDependency
    {
        DateTime? GetMaxDateTime(int bookId, string source);

        bool Any(Expression<Func<BookChapterLink, bool>> expression);

        BookChapterLinkDto Create(BookChapterLinkDto bookChapterLink);

    }
}