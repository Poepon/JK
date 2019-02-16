using Abp.Auditing;
using Abp.AutoMapper;
using Abp.Dependency;
using Abp.Json;
using Abp.RealTime;
using Abp.Runtime.Caching.Redis;
using Abp.Runtime.Session;
using Abp.Timing;
using JK.Chat.Dto;
using JK.Chat.Dto.Input;
using JK.Chat.Dto.Output;
using MessagePack;
using StackExchange.Redis;
using System;
using System.IO;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace JK.Chat
{
    public class ChatHandler : WebSocketHandler, ISingletonDependency
    {
        private readonly IChatAppService chatAppService;
        private readonly IAbpRedisCacheDatabaseProvider databaseProvider;

        protected IOnlineClientManager<ChatChannel> OnlineClientManager { get; }
        protected IClientInfoProvider ClientInfoProvider { get; }
        public IAbpSession AbpSession { get; set; }

        public ChatHandler(WebSocketConnectionManager webSocketConnectionManager,
            IOnlineClientManager<ChatChannel> onlineClientManager,
            IClientInfoProvider clientInfoProvider,
            IChatAppService chatAppService,
            IAbpRedisCacheDatabaseProvider databaseProvider) : base(webSocketConnectionManager)
        {
            this.OnlineClientManager = onlineClientManager;
            ClientInfoProvider = clientInfoProvider;
            this.chatAppService = chatAppService;
            this.databaseProvider = databaseProvider;
            AbpSession = NullAbpSession.Instance;
        }

        public override async Task ReceiveBinaryAsync(WebSocket socket, WebSocketReceiveResult result, BinaryMessage receivedMessage)
        {
            switch (receivedMessage.CommandType)
            {
                case CommandType.Online:
                    var onlineInput = DeserializeFromBytes<OnlineInput>(receivedMessage.DataType, receivedMessage.Data);
                    break;
                case CommandType.Offline:
                    var offlineInput = DeserializeFromBytes<OfflineInput>(receivedMessage.DataType, receivedMessage.Data);
                    break;
                case CommandType.Typing:
                    var typingInput = DeserializeFromBytes<TypingInput>(receivedMessage.DataType, receivedMessage.Data);
                    break;
                case CommandType.SendMessage:
                    {
                        var messageInput = DeserializeFromBytes<SendMessageModel>(receivedMessage.DataType, receivedMessage.Data);
                        await chatAppService.SendMessage(new SendMessageInput
                        {
                            GroupId = messageInput.GroupId,
                            Message = messageInput.Message,
                            UserId = AbpSession.GetUserId()
                        });
                        //trigger publish
                        var lastId = await chatAppService.GetLastReceivedMessageId(new ChatGroupInputBase
                        {
                            GroupId = messageInput.GroupId,
                            UserId = AbpSession.GetUserId()
                        });
                        var oldLastId = lastId;
                        do
                        {
                            var list = await chatAppService.GetNewMessages(new GetNewMessagesInput
                            {
                                GroupId = messageInput.GroupId,
                                UserId = AbpSession.GetUserId(),
                                LastReceivedMessageId = lastId,
                                MaxResultCount = 50,
                                SkipCount = 0,
                                Sorting = "Id"
                            });
                            if (list.TotalCount > 0)
                            {
                                var dtos = list.Items.Select(msg => msg.MapTo<ChatMessageOutput>()).ToArray();
                                await SendMsgPackAsync(socket, CommandType.GetMessage, dtos);
                                lastId = list.Items.Max(x => x.Id);
                            }
                            else
                            {
                                break;
                            }
                        } while (true);
                        if (lastId > oldLastId)
                        {
                            await chatAppService.SetLastReceivedMessageId(new SetLastReceivedIdInput
                            {
                                GroupId = messageInput.GroupId,
                                UserId = AbpSession.GetUserId(),
                                LastReceivedMessageId = lastId
                            });
                        }
                    }
                    break;
                case CommandType.GetMessage:
                    {
                        ChatMessageOutput[] list = null;
                        var getMessageInput = DeserializeFromBytes<GetMessageInput>(receivedMessage.DataType, receivedMessage.Data);
                        if (getMessageInput.MessageId == 0)
                        {
                            var lastId = await chatAppService.GetLastReceivedMessageId(new ChatGroupInputBase
                            {
                                GroupId = getMessageInput.GroupId,
                                UserId = AbpSession.GetUserId()
                            });
                            var oldMessages = await chatAppService.GetOldMessages(new GetOldMessagesInput
                            {
                                MaxResultCount = 100,
                                SkipCount = 0,
                                LastReceivedMessageId = lastId,
                                GroupId = getMessageInput.GroupId,
                                UserId = AbpSession.GetUserId()
                            });

                            var newMessages = await chatAppService.GetNewMessages(new GetNewMessagesInput
                            {
                                MaxResultCount = 100,
                                SkipCount = 0,
                                LastReceivedMessageId = lastId,
                                GroupId = getMessageInput.GroupId,
                                UserId = AbpSession.GetUserId()
                            });
                            list = oldMessages.Items.OrderBy(item => item.Id).Concat(newMessages.Items)
                                .Select(msg => msg.MapTo<ChatMessageOutput>())
                                .ToArray();
                            await SendMsgPackAsync(socket, CommandType.GetMessage, list);
                        }
                        else
                        {

                        }

                    }
                    break;
                case CommandType.PinMessageToTop:
                    var pinMessageToTopInput = DeserializeFromBytes<PinMessageToTopInput>(receivedMessage.DataType, receivedMessage.Data);
                    break;
                case CommandType.UnpinMessageFromTop:
                    var unpinMessageFromTopInput = DeserializeFromBytes<UnpinMessageFromTopInput>(receivedMessage.DataType, receivedMessage.Data);
                    break;
                case CommandType.ReadMessage:
                    var readMessageInput = DeserializeFromBytes<ReadMessageInput>(receivedMessage.DataType, receivedMessage.Data);
                    break;
                case CommandType.CreateGroup:
                    var createGroupInput = DeserializeFromBytes<Dto.Input.CreateGroupInput>(receivedMessage.DataType, receivedMessage.Data);
                    break;
                case CommandType.DeleteGroup:
                    var deleteGroupInput = DeserializeFromBytes<DeleteGroupInput>(receivedMessage.DataType, receivedMessage.Data);
                    break;
                case CommandType.JoinGroup:
                    var joinGroupInput = DeserializeFromBytes<JoinGroupInput>(receivedMessage.DataType, receivedMessage.Data);
                    break;
                case CommandType.LeaveGroup:
                    var leaveGroupInput = DeserializeFromBytes<LeaveGroupInput>(receivedMessage.DataType, receivedMessage.Data);
                    break;
                case CommandType.GetGroups:
                    var getGroupsInput = DeserializeFromBytes<GetGroupsInput>(receivedMessage.DataType, receivedMessage.Data);
                    break;
                case CommandType.PinToTop:
                    var pinToTopInput = DeserializeFromBytes<PinToTopInput>(receivedMessage.DataType, receivedMessage.Data);
                    break;
                case CommandType.UnpinFromTop:
                    var unpinFromTopInput = DeserializeFromBytes<UnpinFromTopInput>(receivedMessage.DataType, receivedMessage.Data);
                    break;
                case CommandType.BlockUser:
                    var blockUserInput = DeserializeFromBytes<BlockUserInput>(receivedMessage.DataType, receivedMessage.Data);
                    break;
                case CommandType.UnblockUser:
                    var unblockUserInput = DeserializeFromBytes<UnblockUserInput>(receivedMessage.DataType, receivedMessage.Data);
                    break;
                case CommandType.UploadFile:
                    var uploadFileInput = DeserializeFromBytes<UploadFileInput>(receivedMessage.DataType, receivedMessage.Data);
                    break;
                case CommandType.DownloadFile:
                    var downloadFileInput = DeserializeFromBytes<DownloadFileInput>(receivedMessage.DataType, receivedMessage.Data);
                    break;
                default:
                    break;
            }
        }

        public override async Task ReceiveTextAsync(WebSocket socket, WebSocketReceiveResult result, string receivedMessage)
        {

        }

        private T DeserializeFromText<T>(MessageDataType dataType, string data) where T : class
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
        private T DeserializeFromBytes<T>(MessageDataType dataType, byte[] data) where T : class
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

        private byte[] SerializeToBytes<T>(MessageDataType dataType, T obj)
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
        private string SerializeToText<T>(MessageDataType dataType, T obj)
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
