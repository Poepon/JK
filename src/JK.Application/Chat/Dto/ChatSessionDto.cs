using Abp.Application.Services.Dto;
using Abp.AutoMapper;

namespace JK.Chat.Dto
{
    [AutoMap(typeof(ChatSession))]
    public class ChatSessionDto : EntityDto<long>
    {
        public string Name { get; set; }

        public ChatSessionType SessionType { get; set; }

        public long CreatorUserId { get; set; }

        public long CreationTime { get; set; }

        public int Status { get; set; }
    }
}
