using System;
using System.Collections.Generic;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;

namespace JK.Books.Dto
{
    [AutoMap(typeof(BookChapter))]
    public class BookChapterDto : EntityDto
    {
        public int BookVolumeId { get; set; }

        /// <summary>
        /// 来源网站
        /// </summary>
        public string Source { get; set; }

        public long SourceChapterId { get; set; }

        /// <summary>
        /// 章节字数
        /// </summary>
        public int? ContentLenght { get; set; }

        /// <summary>
        /// 图书编号
        /// </summary>
        public int BookId { get; set; }

        /// <summary>
        /// 章节标题
        /// </summary>
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
        public string Src { get; set; }

        /// <summary>
        /// 是否免费章节
        /// </summary>
        public bool IsFree { get; set; }

        public DateTime UpdateTime { get; set; }

        public bool IsActive { get; set; } = true;

        public List<BookChapterLinkDto> ChapterLinks { get; set; }

        public int ChapterLinkCount { get; set; }

    }
}