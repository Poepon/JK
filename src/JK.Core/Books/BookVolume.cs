using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;

namespace JK.Books
{
    [Table("BookVolumes")]
    public class BookVolume : Entity,IPassivable
    {
        [Range(1,int.MaxValue)]
        public int BookId { get; set; }
        
        [ForeignKey("BookId")]
        public virtual Book Book { get; set; }

        [StringLength(200)]
        public string Name { get; set; }

        public int Order { get; set; }

        public long QiDianVolumeId { get; set; }

        public bool IsVip { get; set; }

        public bool IsDeleted { get; set; }

        public bool IsActive { get; set; } = true;
    }
}