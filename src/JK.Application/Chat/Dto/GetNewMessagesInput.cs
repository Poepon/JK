using Abp.Application.Services.Dto;
using Abp.Runtime.Validation;

namespace JK.Chat.Dto
{
    public class GetNewMessagesInput : PagedAndSortedResultRequestDto, IShouldNormalize
    {
        public long GroupId { get; set; }

        public long UserId { get; set; }

        public long LastReceivedMessageId { get; set; }

        public void Normalize()
        {
            if (string.IsNullOrEmpty(Sorting))
            {
                Sorting = "Id desc";
            }
        }
    }
}
