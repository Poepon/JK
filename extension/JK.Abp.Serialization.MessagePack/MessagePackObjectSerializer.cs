using System;
using MessagePack;
namespace JK.Abp.Serialization.MessagePack
{
    public class MessagePackObjectSerializer : IObjectSerializer
    {
        public T Deserialize<T>(byte[] bytes)
        {
            return MessagePackSerializer.Deserialize<T>(bytes);
        }

        public object Deserialize(Type type, byte[] bytes)
        {
            return MessagePackSerializer.NonGeneric.Deserialize(type, bytes);
        }

        public byte[] Serialize<T>(T obj)
        {
            return MessagePackSerializer.Serialize(obj);
        }

        public byte[] Serialize(Type type, object obj)
        {
            return MessagePackSerializer.NonGeneric.Serialize(type, obj);
        }
    }
}
