using Abp.Application.Services.Dto;
using Abp.AutoMapper;

namespace JK.Books.Dto
{
    [AutoMapFrom(typeof(BookType))]
    public class BookTypeDto : EntityDto
    {
        public string Name { get; set; }
    }
}
