using System;
using RabbitMQ.Client;

namespace JK.Abp.RabbitMQ
{
    public interface IConnectionPool : IDisposable
    {
        IConnection Get(string connectionName = null);
    }
}