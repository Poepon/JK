using System;
using System.Collections.Generic;
using System.Text;

namespace JK.Chat.Dto
{
    public class BinaryMessage
    {
        public int MessageType { get; set; }

        public int DataLength { get; set; }

        public byte[] Data { get; set; }
    }
}
