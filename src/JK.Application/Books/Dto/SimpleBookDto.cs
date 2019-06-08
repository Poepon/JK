using Abp.Application.Services.Dto;

namespace JK.Books.Dto
{
    public class SimpleBookDto : EntityDto
    {
        public string Name { get; set; }

        public string AuthorName { get; set; }
    }
}