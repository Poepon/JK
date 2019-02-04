using System;
using System.Collections.Generic;
using System.Text;

namespace JK.Chat.Dto
{
    public interface IMessage<T>
    {
        int MessageType { get; set; }

        T Data { get; set; }
    }
}
