using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Linq.Extensions;
using JK.Books.Dto;
using JK.Books.Interfaces;

namespace JK.Books.Implementation
{
    public class BookChapterUnknownLinkAppService : JKAppServiceBase, IBookChapterUnknownLinkAppService
    {
        private readonly IRepository<BookChapterUnknownLink, int> _repository;

        public BookChapterUnknownLinkAppService(IRepository<BookChapterUnknownLink, int> repository)
        {
            _repository = repository;
        }
       
        [RemoteService(false)]
        public PagedResultDto<BookChapterUnknownLinkDto> GetAll(Expression<Func<BookChapterUnknownLink, bool>> expression, PagedAndSortedResultRequestDto input)
        {
            var query = _repository.GetAll();

            query = query.Where(expression);

            var totalCount = query.Count();

            query = query.OrderBy(input.Sorting);
            query = query.PageBy(input);

            var entities = query.ToList();

            return new PagedResultDto<BookChapterUnknownLinkDto>(
                totalCount,
                entities.Select(x => x.MapTo<BookChapterUnknownLinkDto>()).ToList()
            );
        }
        [UnitOfWork]
        public virtual BookChapterUnknownLinkDto Create(BookChapterUnknownLinkDto input)
        {
            BookChapterUnknownLink entity = new BookChapterUnknownLink()
            {
                BookId = input.BookId,
                IsOK = input.IsOK,
                Link = input.Link,
                Source = input.Source,
                SourceId = input.SourceId,
                Title = input.Title
            };
            this._repository.Insert(entity);
            this.CurrentUnitOfWork.SaveChanges();
            return entity.MapTo<BookChapterUnknownLinkDto>();
        }

        public BookChapterUnknownLinkDto Get(EntityDto input)
        {
            return _repository.Get(input.Id).MapTo<BookChapterUnknownLinkDto>();
        }
        [UnitOfWork]
        public virtual BookChapterUnknownLinkDto Update(BookChapterUnknownLinkDto input)
        {
            var entity = _repository.Get(input.Id);
            input.MapTo(entity);
            _repository.Update(entity);
            CurrentUnitOfWork.SaveChanges();
            return entity.MapTo<BookChapterUnknownLinkDto>();
        }
    }
}