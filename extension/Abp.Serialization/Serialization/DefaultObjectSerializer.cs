using MessagePack;
using ProtoBuf;
using System;
using System.Reflection;
using Abp.Dependency;
namespace JK.Serialization
{
    public class DefaultObjectSerializer : IObjectSerializer, ITransientDependency
    {
        public T Deserialize<T>(byte[] bytes)
        {
            var msgpack = typeof(T).GetCustomAttribute<MessagePackObjectAttribute>(false);
            if (msgpack != null)
            {
                return new MessagePackObjectSerializer().Deserialize<T>(bytes);
            }
            var proto = typeof(T).GetCustomAttribute<ProtoContractAttribute>(false);
            if (proto != null)
            {
                return new ProtoObjectSerializer().Deserialize<T>(bytes);
            }
            var json = typeof(T).GetCustomAttribute<JsonObjectAttribute>(false);
            if (json != null)
            {
                return new JsonObjectSerializer().Deserialize<T>(bytes);
            }
            var binary = typeof(T).GetCustomAttribute<SerializableAttribute>(false);
            if (binary != null)
            {
                return (T)BinarySerializationHelper.Deserialize(bytes);
            }

            return new JsonObjectSerializer().Deserialize<T>(bytes); ;
        }

        public object Deserialize(Type type, byte[] bytes)
        {
            var msgpack = type.GetCustomAttribute<MessagePackObjectAttribute>(false);
            if (msgpack != null)
            {
                return new MessagePackObjectSerializer().Deserialize(type, bytes);
            }
            var proto = type.GetCustomAttribute<ProtoContractAttribute>(false);
            if (proto != null)
            {
                return new ProtoObjectSerializer().Deserialize(type, bytes);
            }
            var json = type.GetCustomAttribute<JsonObjectAttribute>(false);
            if (json != null)
            {
                return new JsonObjectSerializer().Deserialize(type, bytes);
            }
            var binary = type.GetCustomAttribute<SerializableAttribute>(false);
            if (binary != null)
            {
                return BinarySerializationHelper.Deserialize(bytes);
            }
            return new JsonObjectSerializer().Deserialize(type, bytes);
        }

        public byte[] Serialize<T>(T obj)
        {
            var msgpack = typeof(T).GetCustomAttribute<MessagePackObjectAttribute>(false);
            if (msgpack != null)
            {
                return new MessagePackObjectSerializer().Serialize<T>(obj);
            }
            var proto = typeof(T).GetCustomAttribute<ProtoContractAttribute>(false);
            if (proto != null)
            {
                return new ProtoObjectSerializer().Serialize<T>(obj);
            }
            var json = typeof(T).GetCustomAttribute<JsonObjectAttribute>(false);
            if (json != null)
            {
                return new JsonObjectSerializer().Serialize<T>(obj);
            }
            var binary = typeof(T).GetCustomAttribute<SerializableAttribute>(false);
            if (binary != null)
            {
                return BinarySerializationHelper.Serialize(obj);
            }
            return new JsonObjectSerializer().Serialize<T>(obj);
        }

        public byte[] Serialize(Type type, object obj)
        {
            var msgpack = type.GetCustomAttribute<MessagePackObjectAttribute>(false);
            if (msgpack != null)
            {
                return new MessagePackObjectSerializer().Serialize(type, obj);
            }
            var proto = type.GetCustomAttribute<ProtoContractAttribute>(false);
            if (proto != null)
            {
                return new ProtoObjectSerializer().Serialize(type, obj);
            }
            var json = type.GetCustomAttribute<JsonObjectAttribute>(false);
            if (json != null)
            {
                return new JsonObjectSerializer().Serialize(type, obj);
            }
            var binary = type.GetCustomAttribute<SerializableAttribute>(false);
            if (binary != null)
            {
                return BinarySerializationHelper.Serialize(obj);
            }
            return new JsonObjectSerializer().Serialize(type, obj);
        }
    }
}
