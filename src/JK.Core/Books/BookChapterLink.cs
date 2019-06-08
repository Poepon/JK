using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;

namespace JK.Books
{
    /// <summary>
    /// 图书章节网络地址
    /// </summary>
    [Table("BookChapterLinks")]
    public class BookChapterLink : Entity
    {
        /// <summary>
        /// 章节编号
        /// </summary>
        [Required]
        [Range(1,int.MaxValue)]
        public int BookChapterId { get; set; }

        [ForeignKey("BookChapterId")]
        public virtual BookChapter BookChapter { get; set; }

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
