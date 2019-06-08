using Abp.Application.Services.Dto;
using Abp.AutoMapper;

namespace JK.Books.Dto
{
    [AutoMap(typeof(Author))]
    public class AuthorDto : EntityDto
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
    }
}
