using System;
using Abp.Application.Services.Dto;
using Abp.Extensions;
using Abp.Runtime.Validation;

namespace JK.Auditing.Dto
{
    public class GetEntityChangeInput : PagedAndSortedResultRequestDto, IShouldNormalize
    {
        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public string UserName { get; set; }

        public string EntityTypeFullName { get; set; }

        public void Normalize()
        {
            if (Sorting.IsNullOrWhiteSpace())
            {
                Sorting = "ChangeTime DESC";
            }

            if (Sorting.IndexOf("UserName", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                Sorting = "User." + Sorting;
            }
            else
            {
                Sorting = "EntityChange." + Sorting;
            }
        }
    }

    public class GetEntityTypeChangeInput : PagedAndSortedResultRequestDto, IShouldNormalize
    {
        public string EntityTypeFullName { get; set; }

        public string EntityId { get; set; }

        public void Normalize()
        {
            if (Sorting.IsNullOrWhiteSpace())
            {
                Sorting = "ChangeTime DESC";
            }

            if (Sorting.IndexOf("UserName", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                Sorting = "User." + Sorting;
            }
            else
            {
                Sorting = "EntityChange." + Sorting;
            }
        }
    }
}