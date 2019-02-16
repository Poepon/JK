using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using System;
using System.ComponentModel.DataAnnotations;

namespace JK.Chat.Dto
{
    [AutoMap(typeof(ChatMessage))]
    public class ChatMessageDto : EntityDto<long>
    {
        public long UserId { get; set; }

        public string UserName { get; set; }

        public long GroupId { get; set; }

        [StringLength(ChatMessage.MaxMessageLength)]
        public string Message { get; set; }

        public DateTime CreationTime { get; set; }

        public ChatMessageReadState ReadState { get; set; }
    }
}
