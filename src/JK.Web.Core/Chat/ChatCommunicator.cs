using Abp;
using Abp.AutoMapper;
using Abp.Dependency;
using Abp.RealTime;
using JK.Chat.Distributed;
using JK.Chat.Dto;
using JK.Chat.Dto.Output;
using JK.Chat.WebSocketPackage;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.ObjectMapping;

namespace JK.Chat
{
    public class ChatCommunicator : IChatCommunicator, ITransientDependency
    {
        private readonly ChatSender _chatSender;
        private readonly IObjectMapper _objectMapper;
        private readonly IOnlineClientManager<ChatChannel> _onlineClientManager;
        private readonly IAppContext _appContext;
        private readonly RedisPubSub _redisPubSub;

        public ChatCommunicator(
            ChatSender chatPost,
            IObjectMapper objectMapper,
            IOnlineClientManager<ChatChannel> onlineClientManager,
            IAppContext appContext, RedisPubSub redisPubSub)
        {
            _chatSender = chatPost;
            _objectMapper = objectMapper;
            _onlineClientManager = onlineClientManager;
            _appContext = appContext;
            _redisPubSub = redisPubSub;
        }

        private bool IsSameServer(IOnlineClient onlineClient)
        {
            return (onlineClient["Server"] as string) == _appContext.LocalHostName;
        }

        private async Task SendMessageToClient(IReadOnlyList<IOnlineClient> clients, ChatMessage message)
        {
            var data = new[] { _objectMapper.Map<ChatMessageOutput>(message) }
                  .WrapPackage(System.Net.WebSockets.WebSocketMessageType.Binary, MessageDataType.MessagePack, CommandType.GetMessage);
            await Send(clients, data);
        }

        private async Task Send(IReadOnlyList<IOnlineClient> clients, byte[] data)
        {
            foreach (var item in clients)
            {
                if (IsSameServer(item))
                {
                    await _chatSender.SendData(item.ConnectionId, data);
                }
                else
                {
                    await _redisPubSub.PublishAsync(item["Server"].ToString(),
                        new ChatQueueDto()
                        {
                            ConnectionId = item.ConnectionId,
                            Data = data
                        });
                }
            }
        }


        private async Task SendMessagesToClient(IReadOnlyList<IOnlineClient> clients, List<ChatMessage> messages)
        {
            var data = _objectMapper.Map<List<ChatMessageOutput>>(messages)
                .WrapPackage(System.Net.WebSockets.WebSocketMessageType.Binary, MessageDataType.MessagePack, CommandType.GetMessage);
            await Send(clients, data);
        }

        public Task SendMessageToUser(IUserIdentifier userId, ChatMessage message)
        {
            var senders = _onlineClientManager.GetAllByUserId(userId);
            return SendMessageToClient(senders, message);
        }

        public Task SendMessagesToUser(IUserIdentifier userId, List<ChatMessage> messages)
        {
            var senders = _onlineClientManager.GetAllByUserId(userId);
            return SendMessagesToClient(senders, messages);
        }

        public Task SendSessionToUser(IUserIdentifier userId, ChatSession session)
        {
            throw new System.NotImplementedException();
        }

        public Task SendSessionsToUser(IUserIdentifier userId, List<ChatSession> sessions)
        {
            throw new System.NotImplementedException();
        }

        public Task SendOnlineClientToUser(IUserIdentifier userId, IOnlineClient onlineClient)
        {
            throw new System.NotImplementedException();
        }

        public Task SendOnlineClientsToUser(IUserIdentifier userId, List<IOnlineClient> onlineClients)
        {
            throw new System.NotImplementedException();
        }
    }
}