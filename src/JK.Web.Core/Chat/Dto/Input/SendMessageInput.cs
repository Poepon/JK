using MessagePack;
using System;
using System.Collections.Generic;
using System.Text;

namespace JK.Chat.Dto.Input
{
    [MessagePackObject]
    public class SendMessageInput
    {
        [Key("gid")]
        public long GroupId { get; set; }

        [Key("uid")]
        public long UserId { get; set; }

        [Key("msg")]
        public string Message { get; set; }
    }
}
