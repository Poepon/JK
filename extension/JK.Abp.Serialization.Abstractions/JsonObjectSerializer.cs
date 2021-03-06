﻿using System;
using System.Text;
using Abp.Dependency;
using Newtonsoft.Json;

namespace JK.Abp.Serialization
{
    public class JsonObjectSerializer : IObjectSerializer, ITransientDependency
    {
        public T Deserialize<T>(byte[] bytes)
        {
            return JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(bytes));
        }

        public object Deserialize(Type type, byte[] bytes)
        {
            return JsonConvert.DeserializeObject(Encoding.UTF8.GetString(bytes), type);
        }

        public byte[] Serialize<T>(T obj)
        {
            return Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(obj));
        }

        public byte[] Serialize(Type type, object obj)
        {
            return Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(obj, type, new JsonSerializerSettings()));
        }
    }
}
