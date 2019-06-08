using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;

namespace JK.Books
{
    /// <summary>
    /// 作者
    /// </summary>
    [Table("Authors")]
    public class Author : Entity
    {
        /// <summary>
        /// 名称
        /// </summary>
        [Required]
        [StringLength(30)]
        public string Name { get; set; }

        public bool IsDeleted { get; set; }
    }
}