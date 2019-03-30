using System.Net.WebSockets;
using System.Threading.Tasks;
using Abp.Dependency;
using JK.Chat.WebSocketPackage;

namespace JK.Chat
{
    public class ChatSender : ITransientDependency
    {
        private readonly WebSocketConnectionManager _connectionManager;
        public ChatSender(WebSocketConnectionManager connectionManager)
        {
            _connectionManager = connectionManager;
        }
        public async Task SendData(string connectionId, byte[] data)
        {
            var client = _connectionManager.GetWebSocketClient(connectionId);
            if (client != null)
            {
                await client.WebSocket.SendAsync(WebSocketMessageType.Binary, data);
            }
        }
    }
}