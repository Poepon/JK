using Abp.Application.Services.Dto;
using Abp.AutoMapper;

namespace JK.Books.Dto
{
    [AutoMap(typeof(BookChapterUnknownLink))]
    public class BookChapterUnknownLinkDto : EntityDto
    {
        public int BookId { get; set; }

        public string Title { get; set; }

        /// <summary>
        /// 来源
        /// </summary>
        public string Source { get; set; }

        /// <summary>
        /// 来源
        /// </summary>
        public string SourceId { get; set; }

        /// <summary>
        /// 链接地址
        /// </summary>
        public string Link { get; set; }

        public bool IsOK { get; set; }
    }
}
