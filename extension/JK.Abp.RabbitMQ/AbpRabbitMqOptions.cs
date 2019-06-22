namespace JK.Abp.RabbitMQ
{
    public class AbpRabbitMqOptions
    {
        public RabbitMqConnections Connections { get; }

        public AbpRabbitMqOptions()
        {
            Connections = new RabbitMqConnections();
        }
    }
}
