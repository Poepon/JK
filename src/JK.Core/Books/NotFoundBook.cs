using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;

namespace JK.Books
{
    [Table("NotFoundBooks")]
    public class NotFoundBook : Entity
    {
        [Required]
        [StringLength(50)]
        public string Source { get; set; }

        public int BookId { get; set; }

        [ForeignKey("BookId")]
        public virtual Book Book { get; set; }
    }
}