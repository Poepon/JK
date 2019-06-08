using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.Timing;
using JK.Authorization.Users;

namespace JK.Books
{
    /// <summary>
    /// 书架
    /// </summary>
    [Table("BookShelfs")]
    public class BookShelf : Entity,IHasCreationTime
    {
        [Range(1,long.MaxValue)]
        public long UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        [Range(1, int.MaxValue)]
        public int BookId { get; set; }

        public virtual Book Book { get; set; }

        public DateTime CreationTime { get; set; } = Clock.Now;
    }
}