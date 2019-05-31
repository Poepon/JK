using System;
using System.Text;
using Abp.Dependency;
using Newtonsoft.Json;

namespace Volo.Abp.RabbitMQ
{
    public class Utf8JsonRabbitMqSerializer : IRabbitMqSerializer, ITransientDependency
    {
        public byte[] Serialize(object obj)
        {
            return Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(obj));
        }

        public object Deserialize(byte[] value, Type type)
        {
            return JsonConvert.DeserializeObject(Encoding.UTF8.GetString(value), type);
        }
    }
}