using Abp.Runtime.Caching.Redis;
using JK.Chat.Dto;
using StackExchange.Redis;
using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Abp.Dependency;
using Abp.Json;
using MessagePack;
using JK.Chat.Dto.Input;
using System.IO;

namespace JK.Chat
{
    public sealed class ChatHandler : WebSocketHandler, ISingletonDependency
    {
        private readonly IAbpRedisCacheDatabaseProvider databaseProvider;

        public ChatHandler(WebSocketConnectionManager webSocketConnectionManager, IAbpRedisCacheDatabaseProvider databaseProvider) : base(webSocketConnectionManager)
        {
            this.databaseProvider = databaseProvider;
        }

        public override Task ReceiveBinaryAsync(WebSocket socket, WebSocketReceiveResult result, BinaryMessage receivedMessage)
        {
            switch (receivedMessage.MessageType)
            {
                case MessageType.Online:
                    var onlineInput = ConvertData<OnlineInput>(receivedMessage.DataType, receivedMessage.Data);
                    break;
                case MessageType.Offline:
                    var offlineInput = ConvertData<OfflineInput>(receivedMessage.DataType, receivedMessage.Data);
                    break;
                case MessageType.Typing:
                    var typingInput = ConvertData<TypingInput>(receivedMessage.DataType, receivedMessage.Data);
                    break;
                case MessageType.SendMessage:
                    var messageInput = ConvertData<SendMessageInput>(receivedMessage.DataType, receivedMessage.Data);
                    break;
                case MessageType.GetMessage:
                    var getMessageInput = ConvertData<GetMessageInput>(receivedMessage.DataType, receivedMessage.Data);
                    break;
                case MessageType.PinMessageToTop:
                    var pinMessageToTopInput = ConvertData<PinMessageToTopInput>(receivedMessage.DataType, receivedMessage.Data);
                    break;
                case MessageType.UnpinMessageFromTop:
                    var unpinMessageFromTopInput = ConvertData<UnpinMessageFromTopInput>(receivedMessage.DataType, receivedMessage.Data);
                    break;
                case MessageType.ReadMessage:
                    var readMessageInput = ConvertData<ReadMessageInput>(receivedMessage.DataType, receivedMessage.Data);
                    break;
                case MessageType.CreateGroup:
                    var createGroupInput = ConvertData<CreateGroupInput>(receivedMessage.DataType, receivedMessage.Data);
                    break;
                case MessageType.DeleteGroup:
                    var deleteGroupInput = ConvertData<DeleteGroupInput>(receivedMessage.DataType, receivedMessage.Data);
                    break;
                case MessageType.JoinGroup:
                    var joinGroupInput = ConvertData<JoinGroupInput>(receivedMessage.DataType, receivedMessage.Data);
                    break;
                case MessageType.LeaveGroup:
                    var leaveGroupInput = ConvertData<LeaveGroupInput>(receivedMessage.DataType, receivedMessage.Data);
                    break;
                case MessageType.PinToTop:
                    var pinToTopInput = ConvertData<PinToTopInput>(receivedMessage.DataType, receivedMessage.Data);
                    break;
                case MessageType.UnpinFromTop:
                    var unpinFromTopInput = ConvertData<UnpinFromTopInput>(receivedMessage.DataType, receivedMessage.Data);
                    break;
                case MessageType.BlockUser:
                    var blockUserInput = ConvertData<BlockUserInput>(receivedMessage.DataType, receivedMessage.Data);
                    break;
                case MessageType.UnblockUser:
                    var unblockUserInput = ConvertData<UnblockUserInput>(receivedMessage.DataType, receivedMessage.Data);
                    break;
                case MessageType.UploadFile:
                    var uploadFileInput = ConvertData<UploadFileInput>(receivedMessage.DataType, receivedMessage.Data);
                    break;
                case MessageType.DownloadFile:
                    var downloadFileInput = ConvertData<DownloadFileInput>(receivedMessage.DataType, receivedMessage.Data);
                    break;
                default:
                    break;
            }

            return Task.CompletedTask;
        }

        private T ConvertData<T>(MessageDataType dataType, string data) where T : class
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
        private T ConvertData<T>(MessageDataType dataType, byte[] data) where T : class
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
                    throw new NotSupportedException("不支持Protobuf。");
                case MessageDataType.Blob:
                    MemoryStream memoryStream = new MemoryStream(data);
                    BinaryReader binaryReader = new BinaryReader(memoryStream);
                    var fileType = (FileType)binaryReader.ReadInt32();
                    break;
                default:
                    throw new NotSupportedException("不支持的数据格式。" + dataType.ToString());
            }
            return value;
        }

        public override Task ReceiveJsonAsync(WebSocket socket, WebSocketReceiveResult result, TextMessage receivedMessage)
        {
            switch (receivedMessage.MessageType)
            {
                case MessageType.Online:
                    var onlineInput = ConvertData<OnlineInput>(receivedMessage.DataType, receivedMessage.Data);
                    break;
                case MessageType.Offline:
                    var offlineInput = ConvertData<OfflineInput>(receivedMessage.DataType, receivedMessage.Data);
                    break;
                case MessageType.Typing:
                    var typingInput = ConvertData<TypingInput>(receivedMessage.DataType, receivedMessage.Data);
                    break;
                case MessageType.SendMessage:
                    var messageInput = ConvertData<SendMessageInput>(receivedMessage.DataType, receivedMessage.Data);
                    break;
                case MessageType.GetMessage:
                    var getMessageInput = ConvertData<GetMessageInput>(receivedMessage.DataType, receivedMessage.Data);
                    break;
                case MessageType.PinMessageToTop:
                    var pinMessageToTopInput = ConvertData<PinMessageToTopInput>(receivedMessage.DataType, receivedMessage.Data);
                    break;
                case MessageType.UnpinMessageFromTop:
                    var unpinMessageFromTopInput = ConvertData<UnpinMessageFromTopInput>(receivedMessage.DataType, receivedMessage.Data);
                    break;
                case MessageType.ReadMessage:
                    var readMessageInput = ConvertData<ReadMessageInput>(receivedMessage.DataType, receivedMessage.Data);
                    break;
                case MessageType.CreateGroup:
                    var createGroupInput = ConvertData<CreateGroupInput>(receivedMessage.DataType, receivedMessage.Data);
                    break;
                case MessageType.DeleteGroup:
                    var deleteGroupInput = ConvertData<DeleteGroupInput>(receivedMessage.DataType, receivedMessage.Data);
                    break;
                case MessageType.JoinGroup:
                    var joinGroupInput = ConvertData<JoinGroupInput>(receivedMessage.DataType, receivedMessage.Data);
                    break;
                case MessageType.LeaveGroup:
                    var leaveGroupInput = ConvertData<LeaveGroupInput>(receivedMessage.DataType, receivedMessage.Data);
                    break;
                case MessageType.PinToTop:
                    var pinToTopInput = ConvertData<PinToTopInput>(receivedMessage.DataType, receivedMessage.Data);
                    break;
                case MessageType.UnpinFromTop:
                    var unpinFromTopInput = ConvertData<UnpinFromTopInput>(receivedMessage.DataType, receivedMessage.Data);
                    break;
                case MessageType.BlockUser:
                    var blockUserInput = ConvertData<BlockUserInput>(receivedMessage.DataType, receivedMessage.Data);
                    break;
                case MessageType.UnblockUser:
                    var unblockUserInput = ConvertData<UnblockUserInput>(receivedMessage.DataType, receivedMessage.Data);
                    break;
                case MessageType.UploadFile:
                    var uploadFileInput = ConvertData<UploadFileInput>(receivedMessage.DataType, receivedMessage.Data);
                    break;
                case MessageType.DownloadFile:
                    var downloadFileInput = ConvertData<DownloadFileInput>(receivedMessage.DataType, receivedMessage.Data);
                    break;
                default:
                    break;
            }
            return Task.CompletedTask;
        }

        public override Task OnConnected(string userId, WebSocket socket)
        {

            return base.OnConnected(userId, socket);
        }

        public override Task OnDisconnected(WebSocket socket)
        {
            return base.OnDisconnected(socket);
        }

        public async Task SendBinaryAsync(WebSocket webSocket,
            MessageType messageType,
            MessageDataType dataType,
            byte[] msgdata,
            CancellationToken? cancellationToken = null)
        {
            var packagedata = new byte[msgdata.Length + 10];
            Array.Copy(BitConverter.GetBytes(Convert.ToInt32(messageType)), 0, packagedata, 0, 4);
            Array.Copy(BitConverter.GetBytes(Convert.ToByte(dataType)), 0, packagedata, 4, 2);
            Array.Copy(BitConverter.GetBytes(Convert.ToInt64(msgdata.Length)), 0, packagedata, 6, 4);
            Array.Copy(msgdata, 0, packagedata, 10, msgdata.Length);
            await SendAsync(webSocket, WebSocketMessageType.Binary, packagedata, cancellationToken);
        }

        public async Task SendTextAsync(WebSocket webSocket, MessageType messageType, string msgdata, CancellationToken? cancellationToken = null)
        {
            var packagedata = Encoding.UTF8.GetBytes(msgdata);
            await SendAsync(webSocket, WebSocketMessageType.Text, packagedata, cancellationToken);
        }

        private async Task Subscribe(long groupId, Action<RedisChannel, RedisValue> handler = null)
        {
            var _connection = databaseProvider.GetDatabase().Multiplexer;
            await _connection.GetSubscriber().SubscribeAsync("chatgroup_" + groupId.ToString(), handler);
        }

        private async Task Unsubscribe(long groupId)
        {
            var _connection = databaseProvider.GetDatabase().Multiplexer;
            await _connection.GetSubscriber().UnsubscribeAsync("chatgroup_" + groupId.ToString());
        }
        private async Task UnsubscribeAll()
        {
            var _connection = databaseProvider.GetDatabase().Multiplexer;
            await _connection.GetSubscriber().UnsubscribeAllAsync();
        }
    }
}
