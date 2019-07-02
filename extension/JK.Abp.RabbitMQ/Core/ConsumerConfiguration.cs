namespace JK.Abp.RabbitMQ
{
    public class QOSConfiguration
    {
        public QOSConfiguration(ushort defaultPrefetchCount, bool global = false)
        {
            PrefetchCount = defaultPrefetchCount;
            Global = global;
        }

        public ushort PrefetchCount { get; set; }

        public bool Global { get; set; }

    }
}
