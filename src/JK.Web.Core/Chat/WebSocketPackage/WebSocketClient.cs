using System;
using System.Net.WebSockets;

namespace JK.Chat.WebSocketPackage
{
    public class WebSocketClient
    {
        public WebSocketClient()
        {
            ConnectTime = DateTimeOffset.Now;
        }
        public WebSocketClient(string connectionId, string ipAddress, WebSocket webSocket) : this()
        {
            ConnectionId = connectionId;
            IpAddress = ipAddress;
            WebSocket = webSocket;
        }
        public long? UserId { get; set; }

        public int? TenantId { get; set; }

        public string IpAddress { get; set; }

        public string ConnectionId { get; set; }

        public WebSocket WebSocket { get; set; }

        public WebSocketMessageType MessageType { get; set; }

        public DateTimeOffset ConnectTime { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is WebSocketClient)
            {
                var b = obj as WebSocketClient;
                return this.UserId == b.UserId && this.WebSocket == b.WebSocket;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
