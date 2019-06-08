using System;
using System.Linq.Expressions;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Dependency;
using JK.Books.Dto;

namespace JK.Books.Interfaces
{
    public interface IBookChapterUnknownLinkAppService : IApplicationService, ITransientDependency
    {
        BookChapterUnknownLinkDto Get(EntityDto input);

        PagedResultDto<BookChapterUnknownLinkDto> GetAll(Expression<Func<BookChapterUnknownLink, bool>> expression, PagedAndSortedResultRequestDto input);

        BookChapterUnknownLinkDto Create(BookChapterUnknownLinkDto input);

        BookChapterUnknownLinkDto Update(BookChapterUnknownLinkDto input);
    }
}