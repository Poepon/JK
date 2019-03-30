using System;
using System.IO;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Abp;
using Abp.Auditing;
using Abp.UI;
using JK.Chat.Dto;
using JK.Chat.Dto.Output;
using Microsoft.AspNetCore.Http;

namespace JK.Chat.WebSocketPackage
{
    public class WebSocketManagerMiddleware
    {
        private readonly RequestDelegate _next;
        private WebSocketHandler _webSocketHandler { get; set; }
        protected IClientInfoProvider ClientInfoProvider { get; }

        public WebSocketManagerMiddleware(RequestDelegate next,
            IClientInfoProvider clientInfoProvider,
            WebSocketHandler webSocketHandler)
        {
            _next = next;
            _webSocketHandler = webSocketHandler;
            ClientInfoProvider = clientInfoProvider;
        }

        public async Task Invoke(HttpContext context)
        {
            if (!context.WebSockets.IsWebSocketRequest)
            {
                await _next.Invoke(context);
                return;
            }
            CancellationToken ct = context.RequestAborted;
            var socket = await context.WebSockets.AcceptWebSocketAsync();
            string connectionId = Guid.NewGuid().ToString("N");
            await _webSocketHandler.OnConnected(connectionId, CreateClientForCurrentConnection(connectionId, socket),context);

            await Receive(socket, ct);
        }

        private async Task Receive(WebSocket socket, CancellationToken cancellationToken)
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
                            result = await socket.ReceiveAsync(buffer, cancellationToken);

                            ms.Write(buffer.Array, buffer.Offset, result.Count);
                        }
                        while (!result.EndOfMessage);

                        ms.Seek(0, SeekOrigin.Begin);

                        if (result.MessageType == WebSocketMessageType.Text)
                        {
                            using (var reader = new StreamReader(ms, Encoding.UTF8))
                            {
                                string message = await reader.ReadToEndAsync();
                                await _webSocketHandler.ReceiveTextAsync(socket, result, message);
                            }
                        }
                        else if (result.MessageType == WebSocketMessageType.Binary)
                        {
                            using (BinaryReader binaryReader = new BinaryReader(ms, Encoding.UTF8))
                            {
                                var message = new BinaryMessage
                                {
                                    CommandType = (CommandType)binaryReader.ReadInt32(),
                                    DataType = (MessageDataType)binaryReader.ReadByte(),
                                };
                                var dataLength = binaryReader.ReadInt32();
                                message.Data = binaryReader.ReadBytes(dataLength);
                                await _webSocketHandler.ReceiveBinaryAsync(socket, result, message);
                            }
                        }
                        else if (result.MessageType == WebSocketMessageType.Close)
                        {
                            try
                            {
                                await _webSocketHandler.OnDisconnected(new WebSocketClient()
                                {
                                    WebSocket = socket
                                });
                            }
                            catch (WebSocketException)
                            {
                                throw; //let's not swallow any exception for now
                            }
                        }
                    }
                }
                catch (UserFriendlyException e)
                {
                    var warn = new AlertMessageOutput
                    {
                        Title = "警告",
                        Type = AlertType.Warning,
                        Text = e.Message
                    };
                    await socket.SendMsgPackAsync(CommandType.AlertMessage, warn, cancellationToken);
                }
                catch (AggregateException)
                {
                    break;
                }
                catch (ThreadAbortException)
                {
                    break;
                }
                catch (TaskCanceledException)
                {
                    break;
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (WebSocketException e)
                {
                    if (e.WebSocketErrorCode == WebSocketError.ConnectionClosedPrematurely)
                    {
                        socket.Abort();
                    }
                }
                catch (AbpException e)
                {
                    break;
                }
            }
            await _webSocketHandler.OnDisconnected(new WebSocketClient()
            {
                WebSocket = socket
            });
        }


        protected virtual WebSocketClient CreateClientForCurrentConnection(string connectionId, WebSocket webSocket)
        {
            return new WebSocketClient(
                connectionId,
                GetIpAddressOfClient(),
                webSocket
            );
        }

        protected virtual string GetIpAddressOfClient()
        {
            try
            {
                return ClientInfoProvider.ClientIpAddress;
            }
            catch (Exception ex)
            {
                return "";
            }
        }
    }
}
