using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Dependency;
using JK.Books.Dto;

namespace JK.Books.Interfaces
{
    public interface IBookAppService : IApplicationService, ITransientDependency
    {
        PagedResultDto<SimpleBookDto> GetHasContentCrawlerBooks(PagedResultRequestDto input);

        BookDto Update(BookDto input);

        BookDto Create(CreateBookDto input);

        int Count();

        int Count(Expression<Func<Book, bool>> expression);

        bool Any(Expression<Func<Book, bool>> expression);

        void UpdateCrawlerStatus(int id);

        BookDto Get(Expression<Func<Book, bool>> expression);

        BookDto Get(EntityDto input);

        SimpleBookDto GetSimpleBook(int bookId);

        PagedResultDto<BookDto> GetListForAdmin(GetListForAdminInput input);

        PagedResultDto<BookListDto> GetBookList(Expression<Func<Book, bool>> expression, PagedAndSortedResultRequestDto input);

        PagedResultDto<BookDto> GetBooks(Expression<Func<Book, bool>> expression, PagedAndSortedResultRequestDto input);

        int GetBooksByOthorSourceNoBookLinkCount(string Source);

        PagedResultDto<BookListDto> GetBooksByOthorSourceNoBookLink(BookSourcePagedResultRequestDto input);

        void InsertIgnoreBook(string source, int bookId);


        DateTime GetLastUpdateTime(int bookId);
    }
}
