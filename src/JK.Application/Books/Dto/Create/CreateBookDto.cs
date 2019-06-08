using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Abp.AutoMapper;

namespace JK.Books.Dto
{
    [AutoMapTo(typeof(BookDto))]
    public class CreateBookDto
    {
        /// <summary>
        /// 书名
        /// </summary>
        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        public string AuthorName { get; set; }

        public string BookTypeName { get; set; }

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

        /// <summary>
        /// 简介
        /// </summary>
        [StringLength(500)]
        public string Introduce { get; set; }

        /// <summary>
        /// 最后更新时间
        /// </summary>
        public DateTime LastUpdateDateTime { get; set; } = new DateTime(2000, 1, 1);

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

    }
}
