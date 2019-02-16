using System;
using Abp.Application.Services.Dto;
using Abp.Runtime.Validation;

namespace JK.Logs.Dto
{
    public class GetUserLoginAttemptsInput : PagedAndSortedResultRequestDto, IShouldNormalize
    {
        public long? UserId { get; set; }

        public string UserName { get; set; }

        public int? TenantId { get; set; }

        public DateTime? StartTime { get; set; }

        public DateTime? EndTime { get; set; }

        public void Normalize()
        {
            if (string.IsNullOrEmpty(Sorting))
            {
                Sorting = "Id desc";
            }
        }
    }
}
