using JK.Chat.Dto;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace JK.Chat
{
    public class WebSocketManagerMiddleware
    {
        private readonly RequestDelegate _next;
        private WebSocketHandler _webSocketHandler { get; set; }

        public WebSocketManagerMiddleware(RequestDelegate next, WebSocketHandler webSocketHandler)
        {
            _next = next;
            _webSocketHandler = webSocketHandler;
        }

        public async Task Invoke(HttpContext context)
        {
            if (!context.WebSockets.IsWebSocketRequest)
            {
                await _next.Invoke(context);
                return;
            }

            var socket = await context.WebSockets.AcceptWebSocketAsync();
            string key = Guid.NewGuid().ToString();
            await _webSocketHandler.OnConnected(key, socket);

            await Receive(socket);
        }

        private async Task Receive(WebSocket socket)
        {
            while (socket.State == WebSocketState.Open)
            {
                ArraySegment<byte> buffer = new ArraySegment<byte>(new byte[1024 * 4]);
                WebSocketReceiveResult result = null;
                try
                {
                    using (var ms = new MemoryStream())
                    {
                        do
                        {
                            result = await socket.ReceiveAsync(buffer, CancellationToken.None);

                            ms.Write(buffer.Array, buffer.Offset, result.Count);
                        }
                        while (!result.EndOfMessage);

                        ms.Seek(0, SeekOrigin.Begin);

                        if (result.MessageType == WebSocketMessageType.Text)
                        {
                            using (var reader = new StreamReader(ms, Encoding.UTF8))
                            {
                                string serializedMessage = await reader.ReadToEndAsync();
                                JsonMessage message = JsonConvert.DeserializeObject<JsonMessage>(serializedMessage);
                                await _webSocketHandler.ReceiveJsonAsync(socket, result, message);
                                return;
                            }
                        }
                        else if (result.MessageType == WebSocketMessageType.Binary)
                        {
                            using (BinaryReader binaryReader = new BinaryReader(ms, Encoding.UTF8))
                            {
                                //TODO 处理二进制消息
                                var message = new BinaryMessage
                                {
                                    MessageType = binaryReader.ReadInt32(),
                                    DataLength = binaryReader.ReadInt32()
                                };
                                message.Data = binaryReader.ReadBytes(message.DataLength);

                                await _webSocketHandler.ReceiveBinaryAsync(socket, result, message);
                                return;
                            }
                        }
                        else if (result.MessageType == WebSocketMessageType.Close)
                        {
                            try
                            {
                                await _webSocketHandler.OnDisconnected(socket);
                            }
                            catch (WebSocketException)
                            {
                                throw; //let's not swallow any exception for now
                            }

                            return;
                        }
                    }
                }
                catch (WebSocketException e)
                {
                    if (e.WebSocketErrorCode == WebSocketError.ConnectionClosedPrematurely)
                    {
                        socket.Abort();
                    }
                }
            }

            await _webSocketHandler.OnDisconnected(socket);
        }
    }
}
