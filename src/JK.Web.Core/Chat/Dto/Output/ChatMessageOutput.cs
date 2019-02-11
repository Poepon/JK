using MessagePack;
using System;

namespace JK.Chat.Dto.Output
{
    [MessagePackObject]
    public class ChatMessageOutput
    {
        [Key("gid")]
        public long GroupId { get; set; }
        [Key("uid")]
        public long UserId { get; set; }
        [Key("uname")]
        public string UserName { get; set; }
        [Key("msg")]
        public string Message { get; set; }
        [Key("time")]
        public DateTime CreationTime { get; set; }
    }
}
