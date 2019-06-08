using System;
using System.Collections.Generic;
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
using JK.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace JK.Books.Implementation
{
    [RemoteService(false)]
    public class BookChapterAppService : JKAppServiceBase, IBookChapterAppService
    {
        private readonly IBookChapterRepository _bookChapterRepository;
        private readonly IBookRepository _bookRepository;
        private readonly IBookChapterLinkRepository _bookChapterLinkRepository;
        private readonly IAuthorCache _authorCache;


        public BookChapterAppService(
            IBookChapterRepository bookChapterRepository,
            IBookRepository bookRepository,
            IBookChapterLinkRepository bookChapterLinkRepository,
            IAuthorCache authorCache
            )
        {
            _bookChapterRepository = bookChapterRepository;
            _bookRepository = bookRepository;
            _bookChapterLinkRepository = bookChapterLinkRepository;
            _authorCache = authorCache;
        }

        [RemoteService(false)]
        public int Count(Expression<Func<BookChapter, bool>> expression)
        {
            return _bookChapterRepository.GetAll().AsNoTracking().Count(expression);
        }

        [RemoteService(false)]
        public bool Any(Expression<Func<BookChapter, bool>> expression)
        {
            return _bookChapterRepository.GetAll().Any(expression);
        }

        [RemoteService(false)]
        public BookChapterDto Get(Expression<Func<BookChapter, bool>> expression)
        {
            var chapter = _bookChapterRepository.GetAll().Where(expression).FirstOrDefault();
            return chapter.MapTo<BookChapterDto>();
        }

        public BookChapterDetailDto GetChapterDetail(int chapterId)
        {
            var dto = _bookChapterRepository.GetAll()
                .Where(c => c.Id == chapterId)
                .Select(x => new BookChapterDetailDto
                {
                    Id = x.Id,
                    //AuthorName = x.Book.Author.Name,
                    BookId = x.BookId,
                    //BookName = x.Book.Name,
                    HasSrc = x.HasSrc,
                    IsFree = x.IsFree,
                    Src = x.Src,
                    Title = x.Title,
                    Order = x.Order
                })
                .FirstOrDefault();
            if (dto != null)
            {
                var book = _bookRepository.Get(dto.BookId);
                dto.BookName = book.Name;
                dto.AuthorName = _authorCache.Get(book.AuthorId).Name;
                var pre = _bookChapterRepository.GetAll().Where(c => c.BookId == dto.BookId && c.IsActive && c.Order < dto.Order).OrderBy(c => c.Order).LastOrDefault();
                var next = _bookChapterRepository.GetAll().Where(c => c.BookId == dto.BookId && c.IsActive && c.Order > dto.Order).OrderBy(c => c.Order).FirstOrDefault();
                dto.PrePageChapterId = pre?.Id ?? dto.Id;
                dto.NextPageChapterId = next?.Id ?? dto.Id;
            }
            return dto;
        }
        [RemoteService(false)]
        public PagedResultDto<BookChapterDto> GetAll(Expression<Func<BookChapter, bool>> expression, PagedAndSortedResultRequestDto input)
        {
            var query = _bookChapterRepository.GetAll();

            query = query.Where(expression);

            var totalCount = query.Count();

            query = query.OrderBy(input.Sorting);
            query = query.PageBy(input);

            var entities = query.ToList();

            return new PagedResultDto<BookChapterDto>(
                totalCount,
                entities.Select(x => x.MapTo<BookChapterDto>()).ToList()
            );
        }
        [RemoteService(false)]
        public PagedResultDto<BookChapterIdDto> GetAllId(Expression<Func<BookChapter, bool>> expression, PagedAndSortedResultRequestDto input)
        {
            var query = _bookChapterRepository.GetAll().AsNoTracking();

            query = query.Where(expression);

            var totalCount = query.Count();

            query = query.OrderBy(input.Sorting);
            query = query.PageBy(input);

            var entities = query.Select(c => new BookChapterIdDto
            {
                BookId = c.BookId,
                ChapterId = c.Id
            }).ToList();

            return new PagedResultDto<BookChapterIdDto>(
                totalCount,
                entities
            );
        }


        public List<BookChapterListDto> GetBookChapters(int bookId, int? volumeId = null)
        {
            return _bookChapterRepository.GetAll()
                .Where(c => c.BookId == bookId && c.IsActive)
                .WhereIf(volumeId.HasValue, c => c.BookVolumeId == volumeId)
                .OrderBy(c => c.Order)
                .Select(c => new BookChapterListDto()
                {
                    Id = c.Id,
                    Title = c.Title,
                    BookVolumeId = c.BookVolumeId,
                    HasSrc = c.HasSrc
                }).ToList();
        }

        [UnitOfWork]
        public virtual List<BookChapterDto> Update(List<BookChapterDto> list)
        {
            List<BookChapterDto> result = new List<BookChapterDto>(list.Count);
            foreach (var bookChapterDto in list)
            {

                BookChapter entityById = _bookChapterRepository.Get(bookChapterDto.Id);

                bookChapterDto.MapTo(entityById);
                _bookChapterRepository.Update(entityById);

            }
            CurrentUnitOfWork.SaveChanges();
            return result;
        }

        [UnitOfWork]
        public virtual List<BookChapterDto> Create(List<BookChapterDto> list)
        {
            var result = new List<BookChapterDto>(list.Count);
            foreach (var bookChapterDto in list)
            {
                result.Add(Create(bookChapterDto));
            }
            return result;
        }
        [RemoteService(false)]
        public PagedResultDto<BookChapterDto> GetAllIncludeChapterLink(Expression<Func<BookChapter, bool>> expression, PagedAndSortedResultRequestDto input)
        {
            var query = _bookChapterRepository.GetAll();

            query = query.Where(expression).Include(c => c.ChapterLinks);

            var totalCount = query.Count();

            query = query.OrderBy(input.Sorting);
            query = query.PageBy(input);

            var entities = query.ToList();

            return new PagedResultDto<BookChapterDto>(
                totalCount,
                entities.Select(x =>
                {
                    var dto = x.MapTo<BookChapterDto>();
                    return dto;
                }).ToList()
            );
        }
        [RemoteService(false)]
        public BookChapterDto GetIncludeChapterLink(Expression<Func<BookChapter, bool>> expression)
        {
            var chapter = _bookChapterRepository.GetAll().Where(expression).Include(c => c.ChapterLinks).FirstOrDefault();

            var dto = chapter.MapTo<BookChapterDto>();
            return dto;
        }

        public BookChapterDto Get(int chapterId)
        {
            return _bookChapterRepository.Get(chapterId).MapTo<BookChapterDto>();
        }
        [UnitOfWork]
        public virtual BookChapterDto Update(BookChapterDto bookChapter)
        {
            var entity = _bookChapterRepository.Get(bookChapter.Id);
            bookChapter.MapTo(entity);
            _bookChapterRepository.Update(entity);
            CurrentUnitOfWork.SaveChanges();
            return entity.MapTo<BookChapterDto>();
        }

        [UnitOfWork]
        public virtual BookChapterDto Create(BookChapterDto bookChapter)
        {
            var entity = bookChapter.MapTo<BookChapter>();
            _bookChapterRepository.Insert(entity);
            CurrentUnitOfWork.SaveChanges();
            return entity.MapTo<BookChapterDto>();
        }
    }
}