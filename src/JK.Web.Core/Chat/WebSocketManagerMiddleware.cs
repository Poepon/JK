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


        public WebSocketManagerMiddleware(RequestDelegate next,
                                          WebSocketHandler webSocketHandler)
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
            await _webSocketHandler.OnConnected(key, socket).ConfigureAwait(false);

            await Receive(socket, async (result, data) =>
            {
                if (result.MessageType == WebSocketMessageType.Text)
                {
                    string serializedMessage = Encoding.UTF8.GetString(data);
                    JsonMessage message = JsonConvert.DeserializeObject<JsonMessage>(serializedMessage);
                    await _webSocketHandler.ReceiveJsonAsync(socket, result, message).ConfigureAwait(false);
                    return;
                }
                else if (result.MessageType == WebSocketMessageType.Binary)
                {
                    MemoryStream ms = new MemoryStream(data);
                    BinaryReader binaryReader = new BinaryReader(ms, Encoding.UTF8);

                    //TODO 处理二进制消息
                    var message = new BinaryMessage();
                    message.MessageType = binaryReader.ReadInt32();
                    message.DataLength = binaryReader.ReadInt32();
                    message.Data = binaryReader.ReadBytes(message.DataLength);

                    await _webSocketHandler.ReceiveBinaryAsync(socket, result, message).ConfigureAwait(false);
                    return;
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
            });
        }

        private async Task Receive(WebSocket socket, Action<WebSocketReceiveResult, byte[]> handleMessage)
        {
            while (socket.State == WebSocketState.Open)
            {
                ArraySegment<Byte> buffer = new ArraySegment<byte>(new Byte[1024 * 4]);
                byte[] data = null;
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
                        data = ms.ToArray();
                    }

                    handleMessage(result, data);
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
