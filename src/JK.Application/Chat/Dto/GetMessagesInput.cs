using Abp.Application.Services.Dto;
using Abp.Runtime.Validation;

namespace JK.Chat.Dto
{
    public enum QueryDirection : byte
    {
        /// <summary>
        /// 向后查询,MessageId &gt; @MessageId
        /// </summary>
        New = 1,
        /// <summary>
        /// 向前查询,MessageId &lt; @MessageId
        /// </summary>
        History = 2
    }

    public class GetMessagesInput : PagedAndSortedResultRequestDto, IShouldNormalize
    {
        public long SessionId { get; set; }

        public long UserId { get; set; }

        public long LastReceivedMessageId { get; set; }

        public QueryDirection Direction { get; set; }

        public void Normalize()
        {
            if (string.IsNullOrEmpty(Sorting))
            {
                Sorting = "Id asc";
            }
        }
    }
}
