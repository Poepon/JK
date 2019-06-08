using System.ComponentModel.DataAnnotations;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;

namespace JK.Books.Dto
{
    [AutoMap(typeof(BookChapterLink))]
    public class BookChapterLinkDto : EntityDto
    {
        /// <summary>
        /// 章节编号
        /// </summary>
        public int BookChapterId { get; set; }

        public string BookChapterTitle { get; set; }

        /// <summary>
        /// 来源
        /// </summary>
        [StringLength(50)]
        public string Source { get; set; }

        /// <summary>
        /// 链接地址
        /// </summary>
        [StringLength(500)]
        public string Link { get; set; }

        public bool IsActive { get; set; } = true;

    }
}
