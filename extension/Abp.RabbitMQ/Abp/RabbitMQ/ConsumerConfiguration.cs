namespace Volo.Abp.RabbitMQ
{
    public class ConsumerConfiguration
    {
        public ConsumerConfiguration(ushort defaultPrefetchCount, bool global = false)
        {
            PrefetchCount = defaultPrefetchCount;
            Global = global;
        }

        public ushort PrefetchCount { get; set; }

        public bool Global { get; set; }

    }
}
