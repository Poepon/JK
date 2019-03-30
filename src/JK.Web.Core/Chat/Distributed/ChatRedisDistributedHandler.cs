using Abp.Dependency;
using System.Threading.Tasks;
using MessagePack;
using StackExchange.Redis;

namespace JK.Chat.Distributed
{
    public class ChatRedisDistributedHandler : IDistributedHandler, ISingletonDependency
    {
        private readonly ChatSender _chatSender;

        public ChatRedisDistributedHandler(ChatSender chatSender)
        {
            _chatSender = chatSender;
        }
        public async Task HandleEventAsync(object eventData)
        {
            var bytes = (RedisValue)eventData;
            var dto = MessagePackSerializer.Deserialize<ChatQueueDto>(bytes);
            if (dto != null)
            {
                await _chatSender.SendData(dto.ConnectionId, dto.Data);
            }
        }
    }
}