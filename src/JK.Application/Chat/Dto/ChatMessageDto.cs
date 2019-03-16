using System.ComponentModel.DataAnnotations;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;

namespace JK.Chat.Dto
{
    [AutoMap(typeof(ChatMessage))]
    public class ChatMessageDto : EntityDto<long>
    {
        public long UserId { get; set; }

        public string UserName { get; set; }

        public long SessionId { get; set; }

        [StringLength(ChatMessage.MaxMessageLength)]
        public string Message { get; set; }

        public long CreationTime { get; set; }

        public ChatMessageReadState ReadState { get; set; }
    }
}
