using System;
using System.Collections.Generic;
using System.Text;

namespace JK.Chat.Dto.Input
{
    public class SendMessageInput
    {
        public long GroupId { get; set; }

        public long UserId { get; set; }

        public string Message { get; set; }
    }
}
