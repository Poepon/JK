using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.RabbitMQ;

namespace Abp.RabbitMQ
{
    public static class RabbitMQExtensions
    {

        public static void AddRabbitMQ(this IServiceCollection services, IConfigurationRoot configurationRoot)
        {
            services.Configure<AbpRabbitMqOptions>(configurationRoot.GetSection("RabbitMQ"));
        }
        public static void AddRabbitMQ(this IServiceCollection services, IConfigurationSection configurationSection)
        {
            services.Configure<AbpRabbitMqOptions>(configurationSection);
        }
    }
}
