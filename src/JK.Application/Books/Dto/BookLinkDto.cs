using System;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;

namespace JK.Books.Dto
{
    [AutoMap(typeof(BookLink))]
    public class BookLinkDto : EntityDto
    {
        /// <summary>
        /// 图书编号
        /// </summary>
        public int BookId { get; set; }

        /// <summary>
        /// 图书
        /// </summary>
        public virtual BookDto Book { get; set; }

        /// <summary>
        /// 来源
        /// </summary>
        public string Source { get; set; }

        /// <summary>
        /// 链接地址
        /// </summary>
        public string Link { get; set; }

        public DateTime? LastAccessTime { get; set; }
    }
}
