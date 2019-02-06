using Abp.Runtime.Caching.Redis;
using JK.Chat.Dto;
using StackExchange.Redis;
using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Abp.Dependency;
namespace JK.Chat
{
    public sealed class ChatHandler : WebSocketHandler, ISingletonDependency
    {
        private readonly IAbpRedisCacheDatabaseProvider databaseProvider;

        public ChatHandler(WebSocketConnectionManager webSocketConnectionManager, IAbpRedisCacheDatabaseProvider databaseProvider) : base(webSocketConnectionManager)
        {
            this.databaseProvider = databaseProvider;
        }

        public override async Task ReceiveBinaryAsync(WebSocket socket, WebSocketReceiveResult result, BinaryMessage receivedMessage)
        {

        }

        public override async Task ReceiveJsonAsync(WebSocket socket, WebSocketReceiveResult result, TextMessage receivedMessage)
        {
            await SendTextAsync(socket, MessageType.ReadMessage, receivedMessage.Data);
        }

        public override Task OnConnected(string userId, WebSocket socket)
        {

            return base.OnConnected(userId, socket);
        }

        public override Task OnDisconnected(WebSocket socket)
        {
            return base.OnDisconnected(socket);
        }

        public async Task SendBinaryAsync(WebSocket webSocket, MessageType messageType, byte[] msgdata, CancellationToken? cancellationToken = null)
        {
            var packagedata = new byte[msgdata.Length + 8];
            Array.Copy(BitConverter.GetBytes(Convert.ToInt32(messageType)), 0, packagedata, 0, 4);
            Array.Copy(BitConverter.GetBytes(Convert.ToInt64(msgdata.Length)), 0, packagedata, 4, 4);
            Array.Copy(msgdata, 0, packagedata, 8, msgdata.Length);
            await SendAsync(webSocket, WebSocketMessageType.Binary, packagedata, cancellationToken);
        }

        public async Task SendTextAsync(WebSocket webSocket, MessageType messageType, string msgdata, CancellationToken? cancellationToken = null)
        {
            var packagedata = Encoding.UTF8.GetBytes(msgdata);
            await SendAsync(webSocket, WebSocketMessageType.Text, packagedata, cancellationToken);
        }

        private async Task Subscribe(long groupId, Action<RedisChannel, RedisValue> handler = null)
        {
            var _connection = databaseProvider.GetDatabase().Multiplexer;
            await _connection.GetSubscriber().SubscribeAsync("chatgroup_" + groupId.ToString(), handler);
        }

        private async Task Unsubscribe(long groupId)
        {
            var _connection = databaseProvider.GetDatabase().Multiplexer;
            await _connection.GetSubscriber().UnsubscribeAsync("chatgroup_" + groupId.ToString());
        }
        private async Task UnsubscribeAll()
        {
            var _connection = databaseProvider.GetDatabase().Multiplexer;
            await _connection.GetSubscriber().UnsubscribeAllAsync();
        }
    }
}
