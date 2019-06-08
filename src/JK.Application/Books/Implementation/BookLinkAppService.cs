using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using JK.Books.Dto;
using JK.Books.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace JK.Books.Implementation
{
    public class BookLinkAppService : JKAppServiceBase, IBookLinkAppService
    {
        private readonly IRepository<BookLink, int> _repository;

        public BookLinkAppService(IRepository<BookLink, int> repository)
        {
            _repository = repository;
        }
        public bool Any(Expression<Func<BookLink, bool>> expression)
        {
            return _repository.GetAll().Any(expression);
        }

        [UnitOfWork]
        public virtual BookLinkDto Create(BookLinkDto input)
        {
            var entity = input.MapTo<BookLink>();
            _repository.Insert(entity);
            CurrentUnitOfWork.SaveChanges();
            return entity.MapTo<BookLinkDto>();
        }

        public BookLinkDto Get(int id)
        {
            return _repository.Get(id).MapTo<BookLinkDto>();
        }

        public ListResultDto<BookLinkDto> GetAll(Expression<Func<BookLink, bool>> expression)
        {
            var entityList = _repository.GetAll().Include(link => link.Book).Where(expression).ToList();
            var dtoList = entityList.MapTo<List<BookLinkDto>>();
            return new ListResultDto<BookLinkDto>(dtoList);
        }
        [UnitOfWork]
        public virtual BookLinkDto Update(BookLinkDto input)
        {
            var entity = _repository.Get(input.Id);

            entity.Link = input.Link;
            entity.LastAccessTime = input.LastAccessTime;
            CurrentUnitOfWork.SaveChanges();
            return entity.MapTo<BookLinkDto>();
        }
    }
}
