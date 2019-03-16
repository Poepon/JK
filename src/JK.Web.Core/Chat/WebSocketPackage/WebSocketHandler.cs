using System;
using System.IO;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Abp.Json;
using JK.Chat.Dto;
using JK.Chat.Dto.Input;
using MessagePack;

namespace JK.Chat.WebSocketPackage
{
    public abstract class WebSocketHandler
    {
        protected WebSocketConnectionManager WebSocketConnectionManager { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="WebSocketHandler"/> class.
        /// </summary>
        /// <param name="webSocketConnectionManager">The web socket connection manager.</param>
        protected WebSocketHandler(WebSocketConnectionManager webSocketConnectionManager)
        {
            WebSocketConnectionManager = webSocketConnectionManager;
        }

        /// <summary>
        /// Called when a client has connected to the server.
        /// </summary>
        /// <param name="client">The web-socket of the client.</param>
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
        public virtual async Task OnDisconnected(WebSocketClient client)
        {
            var connectionId = WebSocketConnectionManager.GetConnectionId(client);
            await WebSocketConnectionManager.RemoveClient(connectionId);
        }


        public async Task SendBinaryAsync(WebSocket webSocket,
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

        public async Task SendMsgPackAsync<T>(WebSocket webSocket, CommandType commandType, T dto, CancellationToken? cancellationToken = null)
        {
            var data = SerializeToBytes(MessageDataType.MessagePack, dto);
            await SendBinaryAsync(webSocket, commandType, MessageDataType.MessagePack, data, cancellationToken);
        }

        public async Task SendProtobufAsync<T>(WebSocket webSocket, CommandType commandType, T dto, CancellationToken? cancellationToken = null)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                ProtoBuf.Serializer.Serialize(memoryStream, dto);
                memoryStream.Seek(0, SeekOrigin.Begin);
                var data = memoryStream.ToArray();
                await SendBinaryAsync(webSocket, commandType, MessageDataType.Protobuf, data, cancellationToken);
            }
        }

        public async Task SendTextAsync(WebSocket webSocket,
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

        public async Task SendAsync(WebSocket socket, WebSocketMessageType messageType, byte[] data, CancellationToken? cancellationToken = null)
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

        protected T DeserializeFromText<T>(MessageDataType dataType, string data) where T : class
        {
            T value = default(T);
            switch (dataType)
            {
                case MessageDataType.Text:
                    value = data as T;
                    break;
                case MessageDataType.Json:
                    value = data.FromJsonString<T>();
                    break;
                case MessageDataType.MessagePack:
                case MessageDataType.Protobuf:
                case MessageDataType.Blob:
                default:
                    throw new NotSupportedException("不支持的数据格式。" + dataType.ToString());
            }
            return value;
        }

        protected T DeserializeFromBytes<T>(MessageDataType dataType, byte[] data) where T : class
        {
            T value = default(T);
            switch (dataType)
            {
                case MessageDataType.Text:
                    value = Encoding.UTF8.GetString(data) as T;
                    break;
                case MessageDataType.Json:
                    var jsonString = Encoding.UTF8.GetString(data);
                    value = jsonString.FromJsonString<T>();
                    break;
                case MessageDataType.MessagePack:
                    value = MessagePackSerializer.Deserialize<T>(data);
                    break;
                case MessageDataType.Protobuf:
                    using (var ms = new MemoryStream())
                    {
                        ms.Write(data, 0, data.Length);
                        ms.Seek(0, SeekOrigin.Begin);
                        value = ProtoBuf.Serializer.Deserialize<T>(ms);
                    }
                    break;
                case MessageDataType.Blob:
                    MemoryStream memoryStream = new MemoryStream(data);
                    BinaryReader binaryReader = new BinaryReader(memoryStream);
                    var fileType = (FileType)binaryReader.ReadInt32();
                    //TODO 图片、音视频文件处理
                    break;
                default:
                    throw new NotSupportedException("不支持的数据格式。" + dataType.ToString());
            }
            return value;
        }

        protected byte[] SerializeToBytes<T>(MessageDataType dataType, T obj)
        {
            byte[] bytes = null;
            switch (dataType)
            {
                case MessageDataType.Text:
                    bytes = obj.ToString().ToBytes();
                    break;
                case MessageDataType.Json:
                    bytes = obj.ToJsonString().ToBytes();
                    break;
                case MessageDataType.MessagePack:
                    bytes = MessagePackSerializer.Serialize(obj);
                    break;
                case MessageDataType.Protobuf:
                    using (var ms = new MemoryStream())
                    {
                        ProtoBuf.Serializer.Serialize(ms, obj);
                        ms.Seek(0, SeekOrigin.Begin);
                        bytes = ms.ToArray();
                    }
                    break;
                case MessageDataType.Blob:

                    break;
                default:
                    break;
            }
            return bytes;
        }

        protected string SerializeToText<T>(MessageDataType dataType, T obj)
        {
            //TODO
            string value = "";
            switch (dataType)
            {
                case MessageDataType.Text:
                    value = obj.ToString();
                    break;
                case MessageDataType.Json:
                    value = obj.ToJsonString();
                    break;
                case MessageDataType.MessagePack:
                    var bytes = MessagePackSerializer.Serialize(obj);
                    value = bytes.GetString();
                    break;
                case MessageDataType.Protobuf:
                    using (var ms = new MemoryStream())
                    {
                        ProtoBuf.Serializer.Serialize(ms, obj);
                        ms.Seek(0, SeekOrigin.Begin);
                        bytes = ms.ToArray();
                        value = bytes.GetString();
                    }
                    break;
                case MessageDataType.Blob:
                    break;
                default:
                    break;
            }
            return value;
        }

        public abstract Task ReceiveTextAsync(WebSocket socket, WebSocketReceiveResult result, string receivedMessage);

        public abstract Task ReceiveBinaryAsync(WebSocket socket, WebSocketReceiveResult result, BinaryMessage receivedMessage);


    }
}
