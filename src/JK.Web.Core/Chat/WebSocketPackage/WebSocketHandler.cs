using JK.Chat.Dto;
using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
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
        public virtual Task OnConnected(string userId, WebSocket socket)
        {
            WebSocketConnectionManager.AddSocket(userId, socket);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Called when a client has disconnected from the server.
        /// </summary>
        /// <param name="socket">The web-socket of the client.</param>
        /// <returns>Awaitable Task.</returns>
        public virtual async Task OnDisconnected(WebSocket socket)
        {
            var socketId = WebSocketConnectionManager.GetId(socket);
            await WebSocketConnectionManager.RemoveSocket(socketId);
        }

        public async Task SendAsync(WebSocket socket, WebSocketMessageType messageType, byte[] data, CancellationToken? cancellationToken = null)
        {
            await socket.SendAsync(new ArraySegment<byte>(data, 0, data.Length), messageType, true, cancellationToken ?? CancellationToken.None);
        }

        public abstract Task ReceiveJsonAsync(WebSocket socket, WebSocketReceiveResult result, TextMessage receivedMessage);

        public abstract Task ReceiveBinaryAsync(WebSocket socket, WebSocketReceiveResult result, BinaryMessage receivedMessage);


    }
}
