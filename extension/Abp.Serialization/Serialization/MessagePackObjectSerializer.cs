using MessagePack;
using System;
using System.Reflection;

namespace JK.Serialization
{
    public class MessagePackObjectSerializer : IObjectSerializer
    {
        public T Deserialize<T>(byte[] bytes)
        {
            var msgpack = typeof(T).GetCustomAttribute<MessagePackObjectAttribute>(false);
            if (msgpack == null)
                throw new NotSupportedException();
            return MessagePackSerializer.Deserialize<T>(bytes);
        }

        public object Deserialize(Type type, byte[] bytes)
        {
            var msgpack = type.GetCustomAttribute<MessagePackObjectAttribute>(false);
            if (msgpack == null)
                throw new NotSupportedException();
            return MessagePackSerializer.NonGeneric.Deserialize(type, bytes);
        }

        public byte[] Serialize<T>(T obj)
        {
            var msgpack = typeof(T).GetCustomAttributes(typeof(MessagePackObjectAttribute), false);
            if (msgpack == null)
                throw new NotSupportedException();
            return MessagePackSerializer.Serialize<T>(obj);
        }

        public byte[] Serialize(Type type, object obj)
        {
            return MessagePackSerializer.NonGeneric.Serialize(type, obj);
        }
    }
}
