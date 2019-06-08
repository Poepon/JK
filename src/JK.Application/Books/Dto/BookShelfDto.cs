using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using AutoMapper;
using JK.Users.Dto;

namespace JK.Books.Dto
{
    [AutoMap(typeof(BookShelf))]
    public class BookShelfDto:EntityDto
    {
        public long UserId { get; set; }

        public virtual UserDto User { get; set; }

        public int BookId { get; set; }

        public virtual BookDto Book { get; set; }
    }
}
