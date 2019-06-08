using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Dependency;
using JK.Books.Dto;

namespace JK.Books.Interfaces
{
    public interface IBookChapterAppService : IApplicationService, ITransientDependency
    {
        BookChapterDto Get(int chapterId);
        BookChapterDto Get(Expression<Func<BookChapter, bool>> expression);

        BookChapterDto GetIncludeChapterLink(Expression<Func<BookChapter, bool>> expression);

        List<BookChapterListDto> GetBookChapters(int bookId, int? volumeId = null);

        int Count(Expression<Func<BookChapter, bool>> expression);

        bool Any(Expression<Func<BookChapter, bool>> expression);

        BookChapterDetailDto GetChapterDetail(int chapterId);

        PagedResultDto<BookChapterDto> GetAll(Expression<Func<BookChapter, bool>> expression, PagedAndSortedResultRequestDto input);

        PagedResultDto<BookChapterIdDto> GetAllId(Expression<Func<BookChapter, bool>> expression, PagedAndSortedResultRequestDto input);

        PagedResultDto<BookChapterDto> GetAllIncludeChapterLink(Expression<Func<BookChapter, bool>> expression, PagedAndSortedResultRequestDto input);

        List<BookChapterDto> Update(List<BookChapterDto> list);

        BookChapterDto Update(BookChapterDto bookChapter);

        List<BookChapterDto> Create(List<BookChapterDto> list);

        BookChapterDto Create(BookChapterDto bookChapter);
    }
}