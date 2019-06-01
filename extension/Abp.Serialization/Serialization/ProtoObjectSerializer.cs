using ProtoBuf;
using System;
using System.IO;
using System.Reflection;

namespace JK.Serialization
{
    public class ProtoObjectSerializer : IObjectSerializer
    {
        public T Deserialize<T>(byte[] bytes)
        {
            var proto = typeof(T).GetCustomAttribute<ProtoContractAttribute>(false);
            if (proto == null)
                throw new NotSupportedException();
            using (var memory = new MemoryStream())
            {
                memory.Write(bytes, 0, bytes.Length);
                memory.Seek(0, SeekOrigin.Begin);
                return Serializer.Deserialize<T>(memory);
            }
        }

        public object Deserialize(Type type, byte[] bytes)
        {
            var proto = type.GetCustomAttribute<ProtoContractAttribute>(false);
            if (proto == null)
                throw new NotSupportedException();
            using (var memory = new MemoryStream())
            {
                memory.Write(bytes, 0, bytes.Length);
                memory.Seek(0, SeekOrigin.Begin);
                return Serializer.Deserialize(type, memory);
            }
        }

        public byte[] Serialize<T>(T obj)
        {
            var proto = typeof(T).GetCustomAttribute<ProtoContractAttribute>(false);
            if (proto == null)
                throw new NotSupportedException();
            using (var memory = new MemoryStream())
            {
                Serializer.Serialize(memory, obj);
                memory.Seek(0, SeekOrigin.Begin);
                return memory.ToArray();
            }
        }

        public byte[] Serialize(Type type, object obj)
        {
            var proto = type.GetCustomAttribute<ProtoContractAttribute>(false);
            if (proto == null)
                throw new NotSupportedException();
            using (var memory = new MemoryStream())
            {
                Serializer.Serialize(memory, obj);
                memory.Seek(0, SeekOrigin.Begin);
                return memory.ToArray();
            }
        }
    }
}
