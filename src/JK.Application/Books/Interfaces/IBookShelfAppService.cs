using System;
using System.Linq.Expressions;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using JK.Books.Dto;

namespace JK.Books.Interfaces
{
    public interface IBookShelfAppService : ICrudAppService<BookShelfDto, int, PagedResultRequestDto>
    {

        bool Any(Expression<Func<BookShelf, bool>> expression);


        void Delete(long userId, int bookId);

        PagedResultDto<BookListDto> GetBooks(long userId, PagedResultRequestDto input);
    }


}
