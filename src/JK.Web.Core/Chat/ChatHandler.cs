using Abp;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Abp.Dependency;
using Abp.Runtime.Caching.Redis;
using Abp.Runtime.Session;
using Abp.Threading;
using Abp.UI;
using JK.Chat.Dto;
using JK.Chat.Dto.Input;
using JK.Chat.Dto.Output;
using Microsoft.AspNetCore.Http;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace JK.Chat
{

    public class ChatHandler : WebSocketHandler, ISingletonDependency
    {
        private readonly IHttpContextAccessor httpContextAccess;
        private readonly IChatAppService chatAppService;
        private readonly IAbpRedisCacheDatabaseProvider databaseProvider;

        public IAbpSession AbpSession { get; set; }

        public ChatHandler(WebSocketConnectionManager webSocketConnectionManager,
            IHttpContextAccessor httpContextAccess,

            IChatAppService chatAppService,
            IAbpRedisCacheDatabaseProvider databaseProvider) : base(webSocketConnectionManager)
        {
            this.httpContextAccess = httpContextAccess;

            this.chatAppService = chatAppService;
            this.databaseProvider = databaseProvider;
            AbpSession = NullAbpSession.Instance;
        }



        public override async Task ReceiveBinaryAsync(WebSocket socket, WebSocketReceiveResult result, BinaryMessage receivedMessage)
        {
            if (!AbpSession.UserId.HasValue)
            {
                throw new AbpException("未登录，请先登录。");
            }
            switch (receivedMessage.CommandType)
            {
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
                        var connections = WebSocketConnectionManager.GetAllFromGroup(messageInput.GroupId.ToString());
                        foreach (var connectionId in connections)
                        {
                            var client = WebSocketConnectionManager.GetWebSocketClient(connectionId);
                            if (client != null)
                            {
                                await SendNewMessage(messageInput.GroupId, client.UserId.GetValueOrDefault(), client.WebSocket);
                            }
                        }
                    }
                    break;
                case CommandType.GetMessage:
                    {
                        ChatMessageOutput[] list = null;
                        var getMessageInput = DeserializeFromBytes<GetMessageInput>(receivedMessage.DataType, receivedMessage.Data);
                        var lastId = await chatAppService.GetLastReceivedMessageId(new ChatGroupInputBase
                        {
                            GroupId = getMessageInput.GroupId,
                            UserId = AbpSession.GetUserId()
                        });
                        var oldLastId = lastId;
                        do
                        {
                            var messages = await chatAppService.GetMessages(new GetMessagesInput
                            {
                                MaxResultCount = getMessageInput.MaxCount,
                                SkipCount = 0,
                                LastReceivedMessageId = lastId,
                                GroupId = getMessageInput.GroupId,
                                UserId = AbpSession.GetUserId(),
                                Direction = getMessageInput.Direction,
                                Sorting = "Id " + (getMessageInput.Direction == QueryDirection.New ? "asc" : "desc")
                            });
                            if (getMessageInput.Direction == QueryDirection.History)
                            {
                                list = messages.Items.OrderBy(msg => msg.Id)
                              .Select(msg => msg.MapTo<ChatMessageOutput>())
                              .ToArray();
                            }
                            else
                            {
                                list = messages.Items
                                 .Select(msg => msg.MapTo<ChatMessageOutput>())
                                 .ToArray();
                            }
                            if (list.Length > 0)
                            {
                                await SendMsgPackAsync(socket, CommandType.GetMessage, list);
                                lastId = getMessageInput.Direction == QueryDirection.New
                                    ? list.Max(x => x.Id)
                                    : list.Min(x => x.Id);
                            }
                            else
                            {
                                break;
                            }
                        } while (getMessageInput.Loop);
                        if (lastId > oldLastId)
                        {
                            await chatAppService.SetLastReceivedMessageId(new SetLastReceivedIdInput
                            {
                                GroupId = getMessageInput.GroupId,
                                UserId = AbpSession.GetUserId(),
                                LastReceivedMessageId = lastId
                            });
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
                case CommandType.CreatePrivate:
                    {
                        var createPrivateInput = DeserializeFromBytes<Dto.Input.CreatePrivateInput>(receivedMessage.DataType, receivedMessage.Data);
                        if (createPrivateInput.TargetUserId != AbpSession.GetUserId())
                        {
                            await chatAppService.CreatePrivate(new Dto.CreatePrivateInput
                            {
                                CreatorUserId = AbpSession.GetUserId(),
                                TargetUserId = createPrivateInput.TargetUserId
                            });
                            var dtos = await GetGroups(AbpSession.GetUserId());
                            await SendMsgPackAsync(socket, CommandType.GetGroups, dtos);
                        }
                        else
                        {
                            throw new UserFriendlyException("不能跟自己发起对话。");
                        }
                    }
                    break;
                case CommandType.CreateGroup:
                    {
                        var createGroupInput = DeserializeFromBytes<Dto.Input.CreateGroupInput>(receivedMessage.DataType, receivedMessage.Data);
                        await chatAppService.CreateGroup(new Dto.CreateGroupInput
                        {
                            GroupName = createGroupInput.GroupName,
                            CreatorUserId = AbpSession.GetUserId()
                        });
                        var dtos = await GetGroups(AbpSession.GetUserId());
                        await SendMsgPackAsync(socket, CommandType.GetGroups, dtos);
                    }
                    break;
                case CommandType.DeleteGroup:
                    var deleteGroupInput = DeserializeFromBytes<Dto.Input.DeleteGroupInput>(receivedMessage.DataType, receivedMessage.Data);
                    await chatAppService.DeleteGroup(new Dto.DeleteGroupInput
                    {
                        GroupId = deleteGroupInput.GroupId,
                        UserId = AbpSession.GetUserId()
                    });
                    //TODO 通知群组成员

                    break;
                case CommandType.JoinGroup:
                    var joinGroupInput = DeserializeFromBytes<JoinGroupInput>(receivedMessage.DataType, receivedMessage.Data);
                    break;
                case CommandType.LeaveGroup:
                    var leaveGroupInput = DeserializeFromBytes<LeaveGroupInput>(receivedMessage.DataType, receivedMessage.Data);
                    break;
                case CommandType.GetGroups:
                    {
                        var dtos = await GetGroups(AbpSession.GetUserId());
                        await SendMsgPackAsync(socket, CommandType.GetGroups, dtos);
                    }
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
                case CommandType.GetOnlineUsers:
                    {
                        var onlineUsers = GetOnlineUsers();
                        await SendMsgPackAsync(socket, CommandType.GetOnlineUsers, onlineUsers, httpContextAccess.HttpContext.RequestAborted);
                    }
                    break;
                default:
                    break;
            }
        }

        public override async Task ReceiveTextAsync(WebSocket socket, WebSocketReceiveResult result, string receivedMessage)
        {
            await Task.CompletedTask;
        }
        public override async Task OnConnected(string connectionId, WebSocketClient client)
        {
            client.UserId = AbpSession.UserId;

            await AddToGroup(client);

            await base.OnConnected(client.ConnectionId, client);

            await SendOnlineUsers();
        }

        public override async Task OnDisconnected(WebSocketClient client)
        {
            client.UserId = AbpSession.UserId;

            await RemoveFromGroup(client);

            await base.OnDisconnected(client);

            await SendOnlineUsers();
        }

        private async Task AddToGroup(WebSocketClient onlineClient)
        {
            var groups = await chatAppService.GetUserGroupsId(new GetUserGroupsInput { UserId = onlineClient.UserId.GetValueOrDefault() });
            foreach (var groupId in groups)
            {
                WebSocketConnectionManager.AddToGroup(onlineClient.ConnectionId, groupId.ToString());
            }
        }

        private async Task RemoveFromGroup(WebSocketClient onlineClient)
        {
            var groups = await chatAppService.GetUserGroupsId(new GetUserGroupsInput { UserId = onlineClient.UserId.GetValueOrDefault() });
            foreach (var groupId in groups)
            {
                WebSocketConnectionManager.RemoveFromGroup(onlineClient.ConnectionId, groupId.ToString());
            }
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

        private async Task SendNewMessage(long groupId, long userId, WebSocket socket)
        {
            var lastId = await chatAppService.GetLastReceivedMessageId(new ChatGroupInputBase
            {
                GroupId = groupId,
                UserId = userId
            });
            var oldLastId = lastId;
            do
            {
                var list = await chatAppService.GetMessages(new GetMessagesInput
                {
                    GroupId = groupId,
                    UserId = userId,
                    LastReceivedMessageId = lastId,
                    MaxResultCount = 100,
                    SkipCount = 0,
                    Direction = QueryDirection.New,
                    Sorting = "Id asc"
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
                    GroupId = groupId,
                    UserId = userId,
                    LastReceivedMessageId = lastId
                });
            }
        }

        private async Task<ChatGroupOutput[]> GetGroups(long userId)
        {
            var groups = await chatAppService.GetUserGroups(new GetUserGroupsInput { UserId = userId });
            var dtos = groups.Items.Select(x =>
            {
                var o = new ChatGroupOutput
                {
                    CreationTime = x.CreationTime,
                    CreatorUserId = x.CreatorUserId,
                    GroupType = x.GroupType,
                    Id = x.Id,
                    Name = x.Name,
                    Status = x.Status,
                    Unread = AsyncHelper.RunSync(() => chatAppService.GetUnreadCount(new ChatGroupInputBase { GroupId = x.Id, UserId = userId })),
                    IsCurrent = false,
                    Icon = "/images/user.png",
                };
                var lstmsg = AsyncHelper.RunSync(() => chatAppService.GetLastMessage(new GetLastMessageInput { GroupId = x.Id }));
                o.LastMessage = lstmsg?.Message;
                o.LastTime = lstmsg?.CreationTime;
                return o;
            }).ToArray();
            return dtos;
        }

        private OnlineUserOutput[] GetOnlineUsers()
        {
            var clients = WebSocketConnectionManager.GetAllClients();
            var onlineUsers = clients.Select(c => new OnlineUserOutput
            {
                UserId = c.UserId.GetValueOrDefault(),
                UserName = AsyncHelper.RunSync(() => chatAppService.GetUserName(new EntityDto<long>(c.UserId.GetValueOrDefault()))),
                Icon = "/images/user.png"
            }).Distinct(new OnlineUserOutputComparer()).ToArray();
            return onlineUsers;
        }

        private async Task SendOnlineUsers()
        {
            var clients = WebSocketConnectionManager.GetAllClients();
            var onlineUsers = GetOnlineUsers();
            foreach (var client in clients)
            {
                await SendMsgPackAsync(client.WebSocket, CommandType.GetOnlineUsers, onlineUsers);
            }
        }
    }

    public class OnlineUserOutputComparer : IEqualityComparer<OnlineUserOutput>
    {

        public bool Equals(Task<OnlineUserOutput> x, Task<OnlineUserOutput> y)
        {
            return x.Result.UserId == y.Result.UserId;
        }

        public bool Equals(OnlineUserOutput x, OnlineUserOutput y)
        {
            return x.UserId == y.UserId;
        }

        public int GetHashCode(Task<OnlineUserOutput> obj)
        {
            return obj.Result.UserId.GetHashCode();
        }

        public int GetHashCode(OnlineUserOutput obj)
        {
            return obj.UserId.GetHashCode();
        }
    }
}
