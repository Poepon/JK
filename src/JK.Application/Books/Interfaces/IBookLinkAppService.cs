using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Dependency;
using JK.Books.Dto;
using System;
using System.Linq.Expressions;

namespace JK.Books.Interfaces
{
    public interface IBookLinkAppService : IApplicationService, ITransientDependency
    {
        BookLinkDto Get(int id);

        ListResultDto<BookLinkDto> GetAll(Expression<Func<BookLink, bool>> expression);

        bool Any(Expression<Func<BookLink, bool>> expression);

        BookLinkDto Create(BookLinkDto input);

        BookLinkDto Update(BookLinkDto input);

    }
}