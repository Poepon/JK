using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;

namespace JK.Books
{
    /// <summary>
    /// 图书种类
    /// </summary>
    [Table("BookTypes")]
    public class BookType : Entity, IPassivable
    {
        public int BookSiteId { get; set; }

        [Required]
        [StringLength(30)]
        public string Name { get; set; }

        public bool IsActive { get; set; } = true;

        public bool IsDeleted { get; set; }

    }
}