using JK.Chat.Dto;
using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace JK.Chat
{
    public abstract class WebSocketHandler
    {
        protected WebSocketConnectionManager WebSocketConnectionManager { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="WebSocketHandler"/> class.
        /// </summary>
        /// <param name="webSocketConnectionManager">The web socket connection manager.</param>
        /// <param name="methodInvocationStrategy">The method invocation strategy used for incoming requests.</param>
        public WebSocketHandler(WebSocketConnectionManager webSocketConnectionManager)
        {
            WebSocketConnectionManager = webSocketConnectionManager;
        }

        /// <summary>
        /// Called when a client has connected to the server.
        /// </summary>
        /// <param name="socket">The web-socket of the client.</param>
        /// <returns>Awaitable Task.</returns>
        public virtual Task OnConnected(string connectionId, WebSocketClient client)
        {
            WebSocketConnectionManager.AddWebSocketClient(connectionId, client);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Called when a client has disconnected from the server.
        /// </summary>
        /// <param name="socket">The web-socket of the client.</param>
        /// <returns>Awaitable Task.</returns>
        public virtual async Task OnDisconnected(WebSocketClient socket)
        {
            var connectionId = WebSocketConnectionManager.GetConnectionId(socket);
            await WebSocketConnectionManager.RemoveClient(connectionId);
        }

        public async Task SendAsync(WebSocket socket, WebSocketMessageType messageType, byte[] data, CancellationToken? cancellationToken = null)
        {
            if (socket != null && socket.State == WebSocketState.Open)
            {
                await socket.SendAsync(new ArraySegment<byte>(data, 0, data.Length), messageType, true, cancellationToken ?? CancellationToken.None);
                Console.WriteLine("send one msg.");
            }
            else
            {
                Console.WriteLine("socket is close.");
            }
        }

        public abstract Task ReceiveTextAsync(WebSocket socket, WebSocketReceiveResult result, string receivedMessage);

        public abstract Task ReceiveBinaryAsync(WebSocket socket, WebSocketReceiveResult result, BinaryMessage receivedMessage);


    }
}
