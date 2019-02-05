using System;
using System.Collections.Generic;
using System.Text;

namespace JK.Chat.Dto
{
    public interface IMessage<T>
    {
        MessageType MessageType { get; set; }

        T Data { get; set; }
    }
}
