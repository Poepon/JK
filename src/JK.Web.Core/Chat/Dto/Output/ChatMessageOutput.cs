using Abp.AutoMapper;
using MessagePack;

namespace JK.Chat.Dto.Output
{
    [MessagePackObject]
    [AutoMapFrom(typeof(ChatMessageDto),typeof(ChatMessage))]
    public class ChatMessageOutput
    {
        [Key("mid")]
        public long Id { get; set; }

        [Key("gid")]
        public long SessionId { get; set; }

        [Key("uid")]
        public long UserId { get; set; }

        [Key("uname")]
        public string SenderName { get; set; }

        [Key("msg")]
        public string Message { get; set; }

        [Key("time")]
        public long CreationTime { get; set; }

        [Key("s")]
        public ChatMessageReadState ReadState { get; set; }
    }
}
