using Abp.Application.Services.Dto;

namespace JK.Books.Dto
{
    public class BookSourcePagedResultRequestDto : PagedAndSortedResultRequestDto
    {
        public string Source { get; set; }
    }
}
