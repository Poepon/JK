using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;

namespace JK.Books
{
    /// <summary>
    /// 图书站（男生站、女生站）
    /// </summary>
    [Table("BookSites")]
    public class BookSite : Entity
    {
        public string Name { get; set; }
    }
}