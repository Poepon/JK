using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;

namespace JK.Books
{
    /// <summary>
    /// 图书网络地址
    /// </summary>
    [Table("BookLinks")]
    public class BookLink : Entity
    {
        /// <summary>
        /// 图书编号
        /// </summary>
        [Required]
        [Range(1,int.MaxValue)]
        public int BookId { get; set; }

        /// <summary>
        /// 图书
        /// </summary>
        [ForeignKey("BookId")]
        public virtual Book Book { get; set; }

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

        public DateTime? LastAccessTime { get; set; }

    }
}
