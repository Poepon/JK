using System;
using System.IO;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Abp.Json;
using JK.Chat.Dto;

namespace JK.Chat.WebSocketPackage
{
    public static class WebSocketExtensions
    {
        public static async Task SendMsgPackAsync<T>(this WebSocket webSocket, CommandType commandType, T dto, CancellationToken? cancellationToken = null)
        {
            var packagedata = dto.WrapPackage(WebSocketMessageType.Binary, MessageDataType.MessagePack, commandType);
            await SendAsync(webSocket, WebSocketMessageType.Binary, packagedata, cancellationToken);
        }

        public static async Task SendProtobufAsync<T>(this WebSocket webSocket, CommandType commandType, T dto, CancellationToken? cancellationToken = null)
        {
            var packagedata = dto.WrapPackage(WebSocketMessageType.Binary, MessageDataType.Protobuf, commandType);
            await SendAsync(webSocket, WebSocketMessageType.Binary, packagedata, cancellationToken);
        }

        public static async Task SendTextAsync(this WebSocket webSocket,
            CommandType commandType,
            MessageDataType dataType,
            string msgdata,
            CancellationToken? cancellationToken = null)
        {
            var message = new TextMessage
            {
                CommandType = commandType,
                DataType = dataType,
                Data = msgdata
            };
            var packagedata = message.ToJsonString().ToBytes();
            await SendAsync(webSocket, WebSocketMessageType.Text, packagedata, cancellationToken);
        }

        public static async Task SendAsync(this WebSocket socket, WebSocketMessageType messageType, byte[] data, CancellationToken? cancellationToken = null)
        {
            if (socket != null && socket.State == WebSocketState.Open)
            {
                await socket.SendAsync(new ArraySegment<byte>(data, 0, data.Length), messageType, true, cancellationToken ?? CancellationToken.None);
            }
            else
            {
                Console.WriteLine("socket is close.");
            }
        }
    }
}