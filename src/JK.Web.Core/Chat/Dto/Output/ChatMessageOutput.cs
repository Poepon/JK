using System;
using System.Collections.Generic;
using System.Text;

namespace JK.Chat.Dto.Output
{
    public class ChatMessageOutput
    {
        public long GroupId { get; set; }

        public long UserId { get; set; }

        public string UserName { get; set; }

        public string Message { get; set; }

        public DateTime CreationTime { get; set; }
    }
}
