using Abp.AutoMapper;
using MessagePack;
using System;

namespace JK.Chat.Dto.Output
{
    [MessagePackObject]
    [AutoMapFrom(typeof(ChatMessageDto))]
    public class ChatMessageOutput
    {
        [Key("mid")]
        public long Id { get; set; }

        [Key("gid")]
        public long GroupId { get; set; }

        [Key("uid")]
        public long UserId { get; set; }

        [Key("uname")]
        public string UserName { get; set; }

        [Key("msg")]
        public string Message { get; set; }

        [Key("time")]
        public long CreationTime { get; set; }

        [Key("s")]
        public ChatMessageReadState ReadState { get; set; }
    }
}
