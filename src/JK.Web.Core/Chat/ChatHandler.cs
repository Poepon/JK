using Abp.Auditing;
using Abp.Dependency;
using Abp.Json;
using Abp.RealTime;
using Abp.Runtime.Caching.Redis;
using Abp.Runtime.Session;
using JK.Chat.Dto;
using JK.Chat.Dto.Input;
using MessagePack;
using StackExchange.Redis;
using System;
using System.IO;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace JK.Chat
{
    public class ChatHandler : WebSocketHandler, ISingletonDependency
    {
        private readonly IAbpRedisCacheDatabaseProvider databaseProvider;

        protected IOnlineClientManager<ChatChannel> OnlineClientManager { get; }
        protected IClientInfoProvider ClientInfoProvider { get; }
        public IAbpSession AbpSession { get; set; }

        public ChatHandler(WebSocketConnectionManager webSocketConnectionManager,
            IOnlineClientManager<ChatChannel> onlineClientManager,
            IClientInfoProvider clientInfoProvider,
            IAbpRedisCacheDatabaseProvider databaseProvider) : base(webSocketConnectionManager)
        {
            this.OnlineClientManager = onlineClientManager;
            ClientInfoProvider = clientInfoProvider;
            this.databaseProvider = databaseProvider;
            AbpSession = NullAbpSession.Instance;
        }

        public override Task ReceiveBinaryAsync(WebSocket socket, WebSocketReceiveResult result, BinaryMessage receivedMessage)
        {
            switch (receivedMessage.CommandType)
            {
                case CommandType.Online:
                    var onlineInput = ConvertData<OnlineInput>(receivedMessage.DataType, receivedMessage.Data);
                    break;
                case CommandType.Offline:
                    var offlineInput = ConvertData<OfflineInput>(receivedMessage.DataType, receivedMessage.Data);
                    break;
                case CommandType.Typing:
                    var typingInput = ConvertData<TypingInput>(receivedMessage.DataType, receivedMessage.Data);
                    break;
                case CommandType.SendMessage:
                    var messageInput = ConvertData<SendMessageInput>(receivedMessage.DataType, receivedMessage.Data);
                    break;
                case CommandType.GetMessage:
                    var getMessageInput = ConvertData<GetMessageInput>(receivedMessage.DataType, receivedMessage.Data);
                    break;
                case CommandType.PinMessageToTop:
                    var pinMessageToTopInput = ConvertData<PinMessageToTopInput>(receivedMessage.DataType, receivedMessage.Data);
                    break;
                case CommandType.UnpinMessageFromTop:
                    var unpinMessageFromTopInput = ConvertData<UnpinMessageFromTopInput>(receivedMessage.DataType, receivedMessage.Data);
                    break;
                case CommandType.ReadMessage:
                    var readMessageInput = ConvertData<ReadMessageInput>(receivedMessage.DataType, receivedMessage.Data);
                    break;
                case CommandType.CreateGroup:
                    var createGroupInput = ConvertData<CreateGroupInput>(receivedMessage.DataType, receivedMessage.Data);
                    break;
                case CommandType.DeleteGroup:
                    var deleteGroupInput = ConvertData<DeleteGroupInput>(receivedMessage.DataType, receivedMessage.Data);
                    break;
                case CommandType.JoinGroup:
                    var joinGroupInput = ConvertData<JoinGroupInput>(receivedMessage.DataType, receivedMessage.Data);
                    break;
                case CommandType.LeaveGroup:
                    var leaveGroupInput = ConvertData<LeaveGroupInput>(receivedMessage.DataType, receivedMessage.Data);
                    break;
                case CommandType.GetGroups:
                    var getGroupsInput = ConvertData<GetGroupsInput>(receivedMessage.DataType, receivedMessage.Data);
                    break;
                case CommandType.PinToTop:
                    var pinToTopInput = ConvertData<PinToTopInput>(receivedMessage.DataType, receivedMessage.Data);
                    break;
                case CommandType.UnpinFromTop:
                    var unpinFromTopInput = ConvertData<UnpinFromTopInput>(receivedMessage.DataType, receivedMessage.Data);
                    break;
                case CommandType.BlockUser:
                    var blockUserInput = ConvertData<BlockUserInput>(receivedMessage.DataType, receivedMessage.Data);
                    break;
                case CommandType.UnblockUser:
                    var unblockUserInput = ConvertData<UnblockUserInput>(receivedMessage.DataType, receivedMessage.Data);
                    break;
                case CommandType.UploadFile:
                    var uploadFileInput = ConvertData<UploadFileInput>(receivedMessage.DataType, receivedMessage.Data);
                    break;
                case CommandType.DownloadFile:
                    var downloadFileInput = ConvertData<DownloadFileInput>(receivedMessage.DataType, receivedMessage.Data);
                    break;
                default:
                    break;
            }

            return Task.CompletedTask;
        }

        public override async Task ReceiveJsonAsync(WebSocket socket, WebSocketReceiveResult result, TextMessage receivedMessage)
        {
            switch (receivedMessage.CommandType)
            {
                case CommandType.Online:
                    var onlineInput = ConvertData<OnlineInput>(receivedMessage.DataType, receivedMessage.Data);
                    break;
                case CommandType.Offline:
                    var offlineInput = ConvertData<OfflineInput>(receivedMessage.DataType, receivedMessage.Data);
                    break;
                case CommandType.Typing:
                    var typingInput = ConvertData<TypingInput>(receivedMessage.DataType, receivedMessage.Data);
                    break;
                case CommandType.SendMessage:
                    var messageInput = ConvertData<SendMessageInput>(receivedMessage.DataType, receivedMessage.Data);
                    await SendAsync(socket, WebSocketMessageType.Text, messageInput.Message.ToBytes());
                    break;
                case CommandType.GetMessage:
                    var getMessageInput = ConvertData<GetMessageInput>(receivedMessage.DataType, receivedMessage.Data);
                    break;
                case CommandType.PinMessageToTop:
                    var pinMessageToTopInput = ConvertData<PinMessageToTopInput>(receivedMessage.DataType, receivedMessage.Data);
                    break;
                case CommandType.UnpinMessageFromTop:
                    var unpinMessageFromTopInput = ConvertData<UnpinMessageFromTopInput>(receivedMessage.DataType, receivedMessage.Data);
                    break;
                case CommandType.ReadMessage:
                    var readMessageInput = ConvertData<ReadMessageInput>(receivedMessage.DataType, receivedMessage.Data);
                    break;
                case CommandType.CreateGroup:
                    var createGroupInput = ConvertData<CreateGroupInput>(receivedMessage.DataType, receivedMessage.Data);
                    break;
                case CommandType.DeleteGroup:
                    var deleteGroupInput = ConvertData<DeleteGroupInput>(receivedMessage.DataType, receivedMessage.Data);
                    break;
                case CommandType.JoinGroup:
                    var joinGroupInput = ConvertData<JoinGroupInput>(receivedMessage.DataType, receivedMessage.Data);
                    break;
                case CommandType.LeaveGroup:
                    var leaveGroupInput = ConvertData<LeaveGroupInput>(receivedMessage.DataType, receivedMessage.Data);
                    break;
                case CommandType.GetGroups:
                    var getGroupsInput = ConvertData<GetGroupsInput>(receivedMessage.DataType, receivedMessage.Data);
                    break;
                case CommandType.PinToTop:
                    var pinToTopInput = ConvertData<PinToTopInput>(receivedMessage.DataType, receivedMessage.Data);
                    break;
                case CommandType.UnpinFromTop:
                    var unpinFromTopInput = ConvertData<UnpinFromTopInput>(receivedMessage.DataType, receivedMessage.Data);
                    break;
                case CommandType.BlockUser:
                    var blockUserInput = ConvertData<BlockUserInput>(receivedMessage.DataType, receivedMessage.Data);
                    break;
                case CommandType.UnblockUser:
                    var unblockUserInput = ConvertData<UnblockUserInput>(receivedMessage.DataType, receivedMessage.Data);
                    break;
                case CommandType.UploadFile:
                    var uploadFileInput = ConvertData<UploadFileInput>(receivedMessage.DataType, receivedMessage.Data);
                    break;
                case CommandType.DownloadFile:
                    var downloadFileInput = ConvertData<DownloadFileInput>(receivedMessage.DataType, receivedMessage.Data);
                    break;
                default:
                    break;
            }
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

        public override async Task OnConnected(string connectionId, WebSocket socket)
        {
            var client = CreateClientForCurrentConnection(connectionId);
            OnlineClientManager.Add(client);

            await base.OnConnected(client.ConnectionId, socket);
            await SendTextAsync(socket, CommandType.Connected, MessageDataType.Json, new { client.ConnectionId }.ToJsonString());
        }

        protected virtual IOnlineClient CreateClientForCurrentConnection(string connectionId)
        {
            return new OnlineClient(
                connectionId,
                GetIpAddressOfClient(),
                AbpSession.TenantId,
                AbpSession.UserId
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

        public override async Task OnDisconnected(WebSocket socket)
        {
            await base.OnDisconnected(socket);
        }

        public async Task SendBinaryAsync(WebSocket webSocket,
            CommandType messageType,
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

        public async Task SendTextAsync(WebSocket webSocket,
            CommandType messageType,
            MessageDataType dataType,
            string msgdata,
            CancellationToken? cancellationToken = null)
        {
            var message = new TextMessage
            {
                CommandType = messageType,
                DataType = dataType,
                Data = msgdata
            };
            var packagedata = message.ToJsonString().ToBytes();
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
