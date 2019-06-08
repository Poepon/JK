using Abp.Application.Services.Dto;
using Abp.Runtime.Validation;

namespace JK.Books.Dto
{
    public class GetListForAdminInput : PagedAndSortedResultRequestDto, IShouldNormalize
    {
        public string BookName { get; set; }

        public BookStatus? BookStatus { get; set; }

        public bool? IsActive { get; set; }

        public bool? IsBest { get; set; }

        public void Normalize()
        {
            if (string.IsNullOrEmpty(Sorting))
            {
                Sorting = "Id";
            }
        }
    }
}
