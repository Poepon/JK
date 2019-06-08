using Abp.Application.Services.Dto;
using Abp.AutoMapper;

namespace JK.Books.Dto
{
    [AutoMapFrom(typeof(BookChapter))]
    public class BookChapterListDto : EntityDto
    {
        /// <summary>
        /// 章节标题
        /// </summary>
        public string Title { get; set; }

        public int BookVolumeId { get; set; }

        public bool HasSrc { get; set; }
    }
}