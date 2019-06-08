using System;
using System.ComponentModel.DataAnnotations;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;

namespace JK.Books.Dto
{
    [AutoMap(typeof(Book),typeof(BookDto))]
    public class EditBookDto : EntityDto
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
        /// 图书种类编号
        /// </summary>
        [Range(1, int.MaxValue)]
        public int BookTypeId { get; set; }


        /// <summary>
        /// 来源
        /// </summary>
        [StringLength(50)]
        public string Source { get; set; }

        /// <summary>
        /// 来源图书ID
        /// </summary>
        [StringLength(20)]
        public string SourceBookId { get; set; }

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

    }
}