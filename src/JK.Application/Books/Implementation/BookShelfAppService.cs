using System;
using System.Linq;
using System.Linq.Expressions;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using JK.Books.Dto;
using JK.Books.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace JK.Books.Implementation
{
    [RemoteService(false)]
    public class BookShelfAppService : CrudAppService<BookShelf, BookShelfDto, int, PagedResultRequestDto>, IBookShelfAppService
    {
        public BookShelfAppService(IRepository<BookShelf, int> repository) : base(repository)
        {

        }
        [RemoteService(false)]
        public bool Any(Expression<Func<BookShelf, bool>> expression)
        {
            return Repository.GetAll().Any(expression);
        }


        public void Delete(long userId, int bookId)
        {
            Repository.Delete(s => s.UserId == userId && s.BookId == bookId);
        }

        public PagedResultDto<BookListDto> GetBooks(long userId, PagedResultRequestDto input)
        {
            var query = Repository.GetAll().Where(s => s.UserId == userId);

            var totalCount = query.Count();

            query = ApplySorting(query, input);
            query = ApplyPaging(query, input);

            var items = query.Include(s => s.Book).ThenInclude(b => b.Author)
                .Select(s => new BookListDto { Id = s.Book.Id,
                    Name = s.Book.Name,
                    ImageUrl = s.Book.ImageSrc,
                    AuthorName = s.Book.Author.Name })
                .ToList();
            return new PagedResultDto<BookListDto>(totalCount, items);
        }
    }
}