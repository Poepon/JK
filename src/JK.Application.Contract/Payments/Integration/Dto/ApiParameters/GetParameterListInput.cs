using Abp.Application.Services.Dto;
using Abp.Runtime.Validation;

namespace JK.Payments.Integration.Dto.ApiParameters
{
    public class GetParameterListInput : PagedAndSortedResultRequestDto, IShouldNormalize
    {
        public void Normalize()
        {
            if (string.IsNullOrEmpty(this.Sorting))
            {
                Sorting = "Id";
            }
        }
    }
}
