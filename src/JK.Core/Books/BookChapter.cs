using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.Timing;

namespace JK.Books
{
    /// <summary>
    /// 图书章节
    /// </summary>
    [Table("BookChapters")]
    public class BookChapter : Entity, IHasCreationTime
    {
        /// <summary>
        /// 图书编号
        /// </summary>
        [Required]
        [Range(1, int.MaxValue)]
        public int BookId { get; set; }

        [ForeignKey("BookId")]
        public virtual Book Book { get; set; }

        /// <summary>
        /// 分卷编号
        /// </summary>
        [Range(1, int.MaxValue)]
        public int BookVolumeId { get; set; }
        
        public virtual ICollection<BookChapterLink> ChapterLinks { get; set; }

        /// <summary>
        /// 来源网站
        /// </summary>
        [StringLength(50)]
        public string Source { get; set; }

        public long SourceChapterId { get; set; }

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

        public DateTime CreationTime { get; set; } = Clock.Now;
    }
}
