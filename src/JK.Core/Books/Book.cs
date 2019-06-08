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
    /// 书
    /// </summary>
    [Table("Books")]
    public class Book : Entity, IPassivable, IHasCreationTime
    {
        /// <summary>
        /// 书名
        /// </summary>
        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        /// <summary>
        /// 别名
        /// </summary>
        [StringLength(50)]
        public string AliasName { get; set; }

        /// <summary>
        /// 作者编号
        /// </summary>
        [Range(1, int.MaxValue)]
        public int AuthorId { get; set; }

        /// <summary>
        /// 作者
        /// </summary>
        [ForeignKey("AuthorId")]
        public virtual Author Author { get; set; }

        /// <summary>
        /// 图书种类编号
        /// </summary>
        [Range(1, int.MaxValue)]
        public int BookTypeId { get; set; }

        /// <summary>
        /// 图书种类
        /// </summary>
        [ForeignKey("BookTypeId")]
        public virtual BookType BookType { get; set; }

        /// <summary>
        /// 来源
        /// </summary>
        [StringLength(50)]
        public string Source { get; set; }

        /// <summary>
        /// 来源图书ID
        /// </summary>
        public long SourceBookId { get; set; }

        /// <summary>
        /// 封面
        /// </summary>
        [StringLength(500)]
        public string ImageSrc { get; set; }

        public bool UseLocalImage { get; set; }

        /// <summary>
        /// 简介
        /// </summary>
        [StringLength(500)]
        public string Introduce { get; set; }

        /// <summary>
        /// 最后更新时间
        /// </summary>
        public DateTime LastUpdateDateTime { get; set; } = new DateTime(2000, 1, 1);

        public DateTime LastCrawlDateTime { get; set; } = new DateTime(2000, 1, 1);

        /// <summary>
        /// 总字数（万字）
        /// </summary>
        public decimal TotalNumberOfWords { get; set; }

        /// <summary>
        /// 图书状态
        /// </summary>
        public BookStatus Status { get; set; }

        /// <summary>
        /// 是否精品 
        /// </summary>
        public bool IsBest { get; set; }

        /// <summary>
        /// 爬虫状态
        /// </summary>
        public CrawlerPriority CrawlerPriority { get; set; } = CrawlerPriority.Default;

        public bool IsActive { get; set; } = true;

        public virtual ICollection<BookLink> BookLinks { get; set; }

        public virtual ICollection<BookChapter> BookChapters { get; set; }

        public DateTime CreationTime { get; set; } = Clock.Now;

        public bool IsDeleted { get; set; }
    }
}