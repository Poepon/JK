using System;
using System.Linq;
using System.Linq.Expressions;
using Abp.Application.Services;
using Abp.AutoMapper;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using JK.Books.Dto;
using JK.Books.Interfaces;

namespace JK.Books.Implementation
{
    public class BookChapterLinkAppService : JKAppServiceBase, IBookChapterLinkAppService
    {
        private readonly IRepository<BookChapterLink, int> Repository;

        public BookChapterLinkAppService(IRepository<BookChapterLink, int> repository)
        {
            Repository = repository;
        }
        [RemoteService(false)]
        public bool Any(Expression<Func<BookChapterLink, bool>> expression)
        {
            return Repository.GetAll().Any(expression);
        }
      

        [UnitOfWork]
        public virtual BookChapterLinkDto Create(BookChapterLinkDto bookChapterLink)
        {
            var entity = bookChapterLink.MapTo<BookChapterLink>();
            Repository.Insert(entity);
            CurrentUnitOfWork.SaveChanges();
            return entity.MapTo<BookChapterLinkDto>();
        }

        public DateTime? GetMaxDateTime(int bookId, string source)
        {
            return DateTime.Now;
        }
    }
}