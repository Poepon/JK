using Abp.Application.Services.Dto;
using Abp.Runtime.Validation;

namespace JK.Chat.Dto
{
    public class GetMessagesInput : PagedAndSortedResultRequestDto, IShouldNormalize
    {
        public long GroupId { get; set; }

        public long UserId { get; set; }

        public void Normalize()
        {
            if (string.IsNullOrEmpty(Sorting))
            {
                Sorting = "Id desc";
            }
        }
    }
}
