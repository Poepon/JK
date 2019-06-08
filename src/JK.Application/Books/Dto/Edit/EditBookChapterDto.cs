using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using System;
using System.ComponentModel.DataAnnotations;

namespace JK.Books.Dto
{
    [AutoMap(typeof(BookChapter))]
    public class EditBookChapterDto : EntityDto
    {
        /// <summary>
        /// 图书编号
        /// </summary>
        [Required]
        [Range(1, int.MaxValue)]
        public int BookId { get; set; }

        /// <summary>
        /// 分卷编号
        /// </summary>
        [Range(1, int.MaxValue)]
        public int BookVolumeId { get; set; }


        /// <summary>
        /// 来源网站
        /// </summary>
        [StringLength(50)]
        public string Source { get; set; }

        /// <summary>
        /// 章节标题
        /// </summary>
        [Required]
        [StringLength(200)]
        public string Title { get; set; }

        /// <summary>
        /// 排序值
        /// </summary>
        public int Order { get; set; }

        /// <summary>
        /// 是否有文本源
        /// </summary>
        public bool HasSrc { get; set; }

        /// <summary>
        /// 源地址
        /// </summary>
        [StringLength(300)]
        public string Src { get; set; }

        /// <summary>
        /// 章节字数
        /// </summary>
        public int? ContentLenght { get; set; }

        /// <summary>
        /// 是否免费章节
        /// </summary>
        public bool IsFree { get; set; }

        public bool IsDeleted { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdateTime { get; set; }

        public bool IsActive { get; set; } = true;

    }
}