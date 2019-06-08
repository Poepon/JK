using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;

namespace JK.Books
{
    [Table("BookChapterUnknownLink")]
    public class BookChapterUnknownLink : Entity
    {
        [Required]
        [Range(1,Int32.MaxValue)]
        public int BookId { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; }

        /// <summary>
        /// 来源
        /// </summary>
        [Required]
        [StringLength(50)]
        public string Source { get; set; }

        /// <summary>
        /// 来源
        /// </summary>
        [StringLength(20)]
        public string SourceId { get; set; }

        /// <summary>
        /// 链接地址
        /// </summary>
        [Required]
        [StringLength(500)]
        public string Link { get; set; }

        public bool IsOK { get; set; }
    }
}
