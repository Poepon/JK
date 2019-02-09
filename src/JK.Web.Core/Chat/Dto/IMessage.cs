using System;
using System.Collections.Generic;
using System.Text;

namespace JK.Chat.Dto
{
    public interface IMessage<T>
    {
        CommandType CommandType { get; set; }

        T Data { get; set; }
    }
}
