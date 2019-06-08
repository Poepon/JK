using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Abp.Domain.Uow;
using Abp.Linq.Extensions;
using Abp.Runtime.Caching;
using JK.Books.Dto;
using JK.Books.Interfaces;
using JK.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace JK.Books.Implementation
{
    public class BookAppService : JKAppServiceBase, IBookAppService
    {
        private readonly ISourceIgnoreBookRepository _ignoreBookRepository;
        private readonly IAuthorCache _authorCache;
        private readonly IBookTypeCache _bookTypeCache;
        private readonly IBookTypeRepository _bookTypeRepository;
        private readonly IAuthorRepository _authorRepository;

        private readonly IBookRepository _bookRepository;

        private readonly ICacheManager _cacheManager;

        public BookAppService(IBookRepository repository,
            IBookTypeRepository bookTypeRepository,
            ISourceIgnoreBookRepository ignoreBookRepository,
            IAuthorCache authorCache,
            IBookTypeCache bookTypeCache,
            IAuthorRepository authorRepository,
            ICacheManager cacheManager)
        {
            _bookTypeRepository = bookTypeRepository;
            _ignoreBookRepository = ignoreBookRepository;
            _authorCache = authorCache;
            _bookTypeCache = bookTypeCache;
            _authorRepository = authorRepository;
            _bookRepository = repository;
            _cacheManager = cacheManager;
        }

        public int Count()
        {
            return _bookRepository.Count();
        }
        [RemoteService(false)]
        public int Count(Expression<Func<Book, bool>> expression)
        {
            return _bookRepository.Count(expression);
        }

        public BookDto Create(CreateBookDto input)
        {
            BookDto dto = input.MapTo<BookDto>();
            if (!string.IsNullOrEmpty(input.BookTypeName))
            {
                var bookType = _bookTypeCache.GetOrNull(input.BookTypeName);
                if (bookType == null)
                {
                    var bookTypeEntity = new BookType() { Name = input.BookTypeName, BookSiteId = 1 };
                    _bookTypeRepository.Insert(bookTypeEntity);
                    CurrentUnitOfWork.SaveChanges();
                    bookType = bookTypeEntity.MapTo<BookTypeDto>();
                }
                dto.BookTypeId = bookType.Id;
            }
            if (!string.IsNullOrEmpty(input.AuthorName))
            {
                var author = _authorCache.GetOrNull(input.AuthorName);
                if (author == null)
                {
                    var authorEntity = _authorRepository.Insert(new Author() { Name = input.AuthorName });
                    CurrentUnitOfWork.SaveChanges();
                    author = authorEntity.MapTo<AuthorDto>();
                }
                dto.AuthorId = author.Id;
            }

            var entity = dto.MapTo<Book>();
            _bookRepository.Insert(entity);
            CurrentUnitOfWork.SaveChanges();
            return entity.MapTo<BookDto>();
        }

        public BookDto Get(EntityDto input)
        {
            var entity = _bookRepository.GetAll().Include(b => b.Author).SingleOrDefault(b => b.Id == input.Id);
            var dto = entity.MapTo<BookDto>();
            return dto;
            // return _cacheManager.GetCache(nameof(BookDto)).Get(input.Id, (() =>
            //{

            //}));
        }
        [RemoteService(false)]
        public BookDto Get(Expression<Func<Book, bool>> expression)
        {
            var entity = _bookRepository.GetAll().Include(b => b.Author).FirstOrDefault(expression);
            var dto = entity.MapTo<BookDto>();
            return dto;
        }

        public int GetBooksByOthorSourceNoBookLinkCount(string Source)
        {
            var linq = from book in _bookRepository.GetAll()
                       join ignoreBook in _ignoreBookRepository.GetAll()
                           on new { BookId = book.Id, Source = Source } equals new
                           {
                               ignoreBook.BookId,
                               ignoreBook.Source
                           } into g
                       from gg in g.DefaultIfEmpty()
                       where !book.BookLinks.Any(link => link.Source == Source)
                             && (gg == null)
                       select book;
            var totalCount = linq.Count();
            return totalCount;
        }

        public PagedResultDto<BookListDto> GetBooksByOthorSourceNoBookLink(BookSourcePagedResultRequestDto input)
        {
            var linq = from book in _bookRepository.GetAllIncluding(b => b.Author)
                       join ignoreBook in _ignoreBookRepository.GetAll()
                           on new { BookId = book.Id, Source = input.Source } equals new
                           {
                               ignoreBook.BookId,
                               ignoreBook.Source
                           } into g
                       from gg in g.DefaultIfEmpty()
                       where !book.BookLinks.Any(link => link.Source == input.Source)
                             && (gg == null)
                       select book;
            var totalCount = linq.Count();

            linq = linq.OrderBy(input.Sorting);
            linq = linq.PageBy(input);

            var entities = linq.Select(x => new BookListDto
            {
                Id = x.Id,
                Name = x.Name,
                ImageUrl = x.ImageSrc,
                AuthorName = x.Author.Name
            }).ToList();
            return new PagedResultDto<BookListDto>(
                totalCount,
                entities
            );
        }

        public PagedResultDto<BookDto> GetListForAdmin(GetListForAdminInput input)
        {
            var query = _bookRepository.GetAll();
            query = query.Include(b => b.Author);
            query = query
                .WhereIf(!string.IsNullOrEmpty(input.BookName), b => b.Name.Contains(input.BookName))
                .WhereIf(input.BookStatus.HasValue, b => b.Status == input.BookStatus)
                .WhereIf(input.IsActive.HasValue, b => b.IsActive == input.IsActive)
                .WhereIf(input.IsBest.HasValue, b => b.IsBest == input.IsBest);
            var totalCount = query.Count();
           
            query = query.OrderBy(input.Sorting);
            query = query.PageBy(input);

            var entities = query.ToList();

            return new PagedResultDto<BookDto>(
                totalCount,
                entities.Select(x =>
                {
                    var dto = x.MapTo<BookDto>();
                    return dto;
                }).ToList()
            );
        }

        [RemoteService(false)]
        public PagedResultDto<BookListDto> GetBookList(Expression<Func<Book, bool>> expression, PagedAndSortedResultRequestDto input)
        {
            var query = _bookRepository.GetAll();

            query = query.Include(b => b.Author).Where(expression);

            var totalCount = query.Count();

            query = query.OrderBy(input.Sorting);
            query = query.PageBy(input);

            var entities = query.Select(x => new BookListDto
            {
                Id = x.Id,
                Name = x.Name,
                UseLocalImage = x.UseLocalImage,
                //ImageUrl = x.UseLocalImage == false ? "/images/default.jpg" : x.ImageSrc,
                AuthorName = x.Author.Name
            }).ToList();

            return new PagedResultDto<BookListDto>(
                totalCount,
                entities
            );
        }

        public void UpdateCrawlerStatus(int id)
        {
            var book = _bookRepository.Get(id);
            book.CrawlerPriority = CrawlerPriority.High;
            _bookRepository.Update(book);
        }

        [RemoteService(false)]
        public bool Any(Expression<Func<Book, bool>> expression)
        {
            return _bookRepository.GetAll().Any(expression);
        }

        public void InsertIgnoreBook(string source, int bookId)
        {
            _ignoreBookRepository.Insert(new NotFoundBook() { BookId = bookId, Source = source });
        }


        public DateTime GetLastUpdateTime(int bookId)
        {
            return _bookRepository.GetLastUpdateTime(bookId).GetValueOrDefault(new DateTime(2000, 1, 1));
        }

        public PagedResultDto<SimpleBookDto> GetHasContentCrawlerBooks(PagedResultRequestDto input)
        {
            var linq = _bookRepository.GetAll()
                  .Where(b => b.BookChapters.Any(c => c.HasSrc == false && c.ChapterLinks.Any()))
                  .Select(b => new SimpleBookDto
                  {
                      Id = b.Id,
                      Name = b.Name
                  });
          
            var totalCount = linq.Count();
            linq = linq.PageBy(input);
            var list = linq.ToList();
            return new PagedResultDto<SimpleBookDto>(totalCount, list);
        }
        [RemoteService(false)]
        public PagedResultDto<BookDto> GetBooks(Expression<Func<Book, bool>> expression, PagedAndSortedResultRequestDto input)
        {
            var query = _bookRepository.GetAll();

            query = query.Include(b => b.Author).Where(expression);

            var totalCount = query.Count();

            query = query.OrderBy(input.Sorting);
            query = query.PageBy(input);

            var entities = query.ToList();

            return new PagedResultDto<BookDto>(
                totalCount,
                entities.MapTo<List<BookDto>>()
            );
        }

        [UnitOfWork]
        public virtual BookDto Update(BookDto input)
        {
            var tempDto = input.MapTo<EditBookDto>();
            var entity = _bookRepository.Get(input.Id);
            tempDto.MapTo(entity);
            _bookRepository.Update(entity);
            CurrentUnitOfWork.SaveChanges();
            return entity.MapTo<BookDto>();
        }

        public SimpleBookDto GetSimpleBook(int bookId)
        {
            var entity = _bookRepository.GetAll().
                Include(b => b.Author)
                .Where(b => b.Id == bookId)
                .Select(b => new SimpleBookDto()
                {
                    Id = b.Id,
                    Name = b.Name,
                    AuthorName = b.Author.Name
                })
                .SingleOrDefault();

            return entity;
        }
    }
}