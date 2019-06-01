﻿using Abp.Dependency;
using System.Threading.Tasks;

namespace Abp.RabbitMQ
{
    public interface IRabbitMQConsumer
    {

    }
    public interface IRabbitMQConsumer<T> : IRabbitMQConsumer
    {
        void Initialize();

        Task ConsumeAsync(T message);
    }
}
