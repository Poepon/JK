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

        public static async Task SendBinaryAsync(this WebSocket webSocket,
            CommandType commandType,
            MessageDataType dataType,
            byte[] msgdata,
            CancellationToken? cancellationToken = null)
        {
            var packagedata = new byte[msgdata.Length + 9];
            Array.Copy(BitConverter.GetBytes(Convert.ToInt32(commandType)), 0, packagedata, 0, 4);
            Array.Copy(BitConverter.GetBytes(Convert.ToByte(dataType)), 0, packagedata, 4, 1);
            Array.Copy(BitConverter.GetBytes(Convert.ToInt64(msgdata.Length)), 0, packagedata, 5, 4);
            Array.Copy(msgdata, 0, packagedata, 9, msgdata.Length);
            await SendAsync(webSocket, WebSocketMessageType.Binary, packagedata, cancellationToken);
        }

        public static async Task SendMsgPackAsync<T>(this WebSocket webSocket, CommandType commandType, T dto, CancellationToken? cancellationToken = null)
        {
            var data = dto.SerializeToBytes(MessageDataType.MessagePack);
            await SendBinaryAsync(webSocket, commandType, MessageDataType.MessagePack, data, cancellationToken);
        }

        public static async Task SendProtobufAsync<T>(this WebSocket webSocket, CommandType commandType, T dto, CancellationToken? cancellationToken = null)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                ProtoBuf.Serializer.Serialize(memoryStream, dto);
                memoryStream.Seek(0, SeekOrigin.Begin);
                var data = memoryStream.ToArray();
                await SendBinaryAsync(webSocket, commandType, MessageDataType.Protobuf, data, cancellationToken);
            }
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

        private static async Task SendAsync(this WebSocket socket, WebSocketMessageType messageType, byte[] data, CancellationToken? cancellationToken = null)
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