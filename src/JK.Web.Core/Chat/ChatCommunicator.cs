using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.AutoMapper;
using Abp.Dependency;
using Abp.RealTime;
using JK.Chat.Dto;
using JK.Chat.Dto.Output;
using JK.Chat.WebSocketPackage;

namespace JK.Chat
{
    public class ChatCommunicator : IChatCommunicator, ITransientDependency
    {
        private readonly WebSocketConnectionManager _connectionManager;

        public ChatCommunicator(WebSocketConnectionManager connectionManager)
        {
            _connectionManager = connectionManager;
        }

        public async Task SendMessageToClient(IReadOnlyList<IOnlineClient> clients, ChatMessage message)
        {
            foreach (var item in clients)
            {
                var client = _connectionManager.GetWebSocketClient(item.ConnectionId);
                if (client != null)
                {
                    var output = message.MapTo<ChatMessageOutput>();
                    await client.WebSocket.SendMsgPackAsync(CommandType.GetMessage, new[] { output });
                }
            }
        }

        public async Task SendMessagesToClient(IReadOnlyList<IOnlineClient> clients, List<ChatMessage> messages)
        {
            foreach (var item in clients)
            {
                var client = _connectionManager.GetWebSocketClient(item.ConnectionId);
                if (client != null)
                {
                    var output = messages.MapTo<List<ChatMessageOutput>>();
                    await client.WebSocket.SendMsgPackAsync(CommandType.GetMessage, output);
                }
            }
        }

        public Task SendSessionToClient(IReadOnlyList<IOnlineClient> clients, ChatSession session)
        {
            throw new System.NotImplementedException();
        }

        public Task SendSessionsToClient(IReadOnlyList<IOnlineClient> clients, List<ChatSession> sessions)
        {
            throw new System.NotImplementedException();
        }

        public Task SendOnlineClientToClient(IReadOnlyList<IOnlineClient> clients, IOnlineClient onlineClient)
        {
            throw new System.NotImplementedException();
        }

        public Task SendOnlineClientsToClient(IReadOnlyList<IOnlineClient> clients, List<IOnlineClient> onlineClients)
        {
            throw new System.NotImplementedException();
        }
    }
}