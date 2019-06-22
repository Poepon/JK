using System;

namespace JK.Abp.Serialization
{
    public interface IObjectSerializer
    {
        byte[] Serialize<T>(T obj);

        byte[] Serialize(Type type, object obj);

        T Deserialize<T>(byte[] bytes);

        object Deserialize(Type type, byte[] bytes);
    }
}
