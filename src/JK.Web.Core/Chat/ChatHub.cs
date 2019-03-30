using Abp;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Abp.Dependency;
using Abp.RealTime;
using Abp.Runtime.Session;
using Abp.Threading;
using Abp.UI;
using JK.Chat.Distributed;
using JK.Chat.Dto;
using JK.Chat.Dto.Input;
using JK.Chat.Dto.Output;
using JK.Chat.WebSocketPackage;
using JK.Runtime.Session;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace JK.Chat
{
    public class ChatHub : OnlineClientHubBase, ISingletonDependency
    {
        private readonly IHttpContextAccessor httpContextAccess;
        private readonly IChatAppService chatAppService;
        private readonly RedisPubSub _redisPubSub;

        private readonly JKSession AbpSession;

        public ChatHub(WebSocketConnectionManager webSocketConnectionManager,
            IHttpContextAccessor httpContextAccess,
            IOnlineClientManager<ChatChannel> onlineClientManager,
            IChatAppService chatAppService,
            JKSession jkSession,
            IAppContext appContext,
            RedisPubSub redisPubSub) :
            base(appContext, onlineClientManager, webSocketConnectionManager)
        {
            this.httpContextAccess = httpContextAccess;

            this.chatAppService = chatAppService;
            AbpSession = jkSession;
            _redisPubSub = redisPubSub;
            _redisPubSub.Subscribe(appContext.LocalHostName);
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
                    var typingInput = receivedMessage.DeserializeFromBytes<TypingInput>();
                    break;
                case CommandType.SendMessage:
                    {
                        var messageInput = receivedMessage.DeserializeFromBytes<SendMessageModel>();
                        await chatAppService.SendMessage(new SendMessageInput
                        {
                            SessionId = messageInput.SessionId,
                            Message = messageInput.Message,
                            UserId = AbpSession.GetUserId(),
                            SenderName = AbpSession.FullName
                        });
                    }
                    break;
                case CommandType.GetMessage:
                    {
                        ChatMessageOutput[] list = null;
                        var getMessageInput = receivedMessage.DeserializeFromBytes<GetMessageInput>();
                        var lastId = await chatAppService.GetLastReceivedMessageId(new ChatSessionInputBase
                        {
                            SessionId = getMessageInput.SessionId,
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
                                SessionId = getMessageInput.SessionId,
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
                                await socket.SendMsgPackAsync(CommandType.GetMessage, list);
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
                                SessionId = getMessageInput.SessionId,
                                UserId = AbpSession.GetUserId(),
                                LastReceivedMessageId = lastId
                            });
                        }
                    }
                    break;
                case CommandType.PinMessageToTop:
                    var pinMessageToTopInput = receivedMessage.DeserializeFromBytes<PinMessageToTopInput>();
                    break;
                case CommandType.UnpinMessageFromTop:
                    var unpinMessageFromTopInput = receivedMessage.DeserializeFromBytes<UnpinMessageFromTopInput>();
                    break;
                case CommandType.ReadMessage:
                    var readMessageInput = receivedMessage.DeserializeFromBytes<ReadMessageInput>();
                    break;
                case CommandType.CreatePrivateSession:
                    {
                        var createPrivateInput = receivedMessage.DeserializeFromBytes<Dto.Input.CreatePrivateSessionInput>();
                        if (createPrivateInput.TargetUserId != AbpSession.GetUserId())
                        {
                            await chatAppService.CreatePrivateSession(new Dto.CreatePrivateSessionInput
                            {
                                CreatorUserId = AbpSession.GetUserId(),
                                TargetUserId = createPrivateInput.TargetUserId
                            });
                            var dtos = await GetSessions(AbpSession.GetUserId());
                            await socket.SendMsgPackAsync(CommandType.GetSessions, dtos);
                        }
                        else
                        {
                            throw new UserFriendlyException("不能跟自己发起对话。");
                        }
                    }
                    break;
                case CommandType.CreatePublicSession:
                    {
                        var createGroupInput = receivedMessage.DeserializeFromBytes<Dto.Input.CreatePublicSessionInput>();
                        await chatAppService.CreatePublicSession(new Dto.CreatePublicSessionInput
                        {
                            SessionName = createGroupInput.SessionName,
                            CreatorUserId = AbpSession.GetUserId()
                        });
                        var dtos = await GetSessions(AbpSession.GetUserId());
                        await socket.SendMsgPackAsync(CommandType.GetSessions, dtos);
                    }
                    break;
                case CommandType.DeleteSession:
                    var deleteGroupInput = receivedMessage.DeserializeFromBytes<Dto.Input.DeleteSessionInput>();
                    await chatAppService.DeleteSession(new Dto.DeleteSessionInput
                    {
                        SessionId = deleteGroupInput.SessionId,
                        UserId = AbpSession.GetUserId()
                    });
                    //TODO 通知群组成员

                    break;
                case CommandType.JoinSession:
                    var joinGroupInput = receivedMessage.DeserializeFromBytes<JoinSessionInput>();
                    break;
                case CommandType.LeaveSession:
                    var leaveGroupInput = receivedMessage.DeserializeFromBytes<LeaveSessionInput>();
                    break;
                case CommandType.GetSessions:
                    {
                        var dtos = await GetSessions(AbpSession.GetUserId());
                        await socket.SendMsgPackAsync(CommandType.GetSessions, dtos);
                    }
                    break;
                case CommandType.GetSessionUnread:
                    {
                        var getGroupUnreadInput = receivedMessage.DeserializeFromBytes<GetSessionUnreadInput>();
                        var count = await chatAppService.GetSessionUnread(new ChatSessionInputBase
                        {
                            SessionId = getGroupUnreadInput.SessionId,
                            UserId = AbpSession.GetUserId()
                        });
                        var output = new SessionUnreadOutput { Count = count };
                        await socket.SendMsgPackAsync(CommandType.GetSessionUnread, output, httpContextAccess.HttpContext.RequestAborted);
                    }
                    break;
                case CommandType.PinToTop:
                    var pinToTopInput = receivedMessage.DeserializeFromBytes<PinToTopInput>();
                    break;
                case CommandType.UnpinFromTop:
                    var unpinFromTopInput = receivedMessage.DeserializeFromBytes<UnpinFromTopInput>();
                    break;
                case CommandType.BlockUser:
                    var blockUserInput = receivedMessage.DeserializeFromBytes<BlockUserInput>();
                    break;
                case CommandType.UnblockUser:
                    var unblockUserInput = receivedMessage.DeserializeFromBytes<UnblockUserInput>();
                    break;
                case CommandType.UploadFile:
                    var uploadFileInput = receivedMessage.DeserializeFromBytes<UploadFileInput>();
                    break;
                case CommandType.DownloadFile:
                    var downloadFileInput = receivedMessage.DeserializeFromBytes<DownloadFileInput>();
                    break;
                case CommandType.GetOnlineUsers:
                    {
                        var onlineUsers = GetOnlineUsers();
                        await socket.SendMsgPackAsync(CommandType.GetOnlineUsers, onlineUsers, httpContextAccess.HttpContext.RequestAborted);
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
        public override async Task OnConnected(string connectionId, WebSocketClient client, HttpContext context)
        {
            client.UserId = AbpSession.UserId;
            client.TenantId = AbpSession.TenantId;

            await AddToGroup(client);

            await base.OnConnected(client.ConnectionId, client, context);

            await SendOnlineUsers(client);
        }

        public override async Task OnDisconnected(WebSocketClient client)
        {
            client.UserId = AbpSession.UserId;
            client.TenantId = AbpSession.TenantId;

            await RemoveFromGroup(client);

            await base.OnDisconnected(client);

            await SendOnlineUsers(client);
        }

        private async Task AddToGroup(WebSocketClient onlineClient)
        {
            var groups = await chatAppService.GetUserSessionsId(new GetUserSessionsInput { UserId = onlineClient.UserId.GetValueOrDefault() });
            foreach (var groupId in groups)
            {
                WebSocketConnectionManager.AddToGroup(onlineClient.ConnectionId, groupId.ToString());
            }
        }

        private async Task RemoveFromGroup(WebSocketClient onlineClient)
        {
            var groups = await chatAppService.GetUserSessionsId(new GetUserSessionsInput { UserId = onlineClient.UserId.GetValueOrDefault() });
            foreach (var groupId in groups)
            {
                WebSocketConnectionManager.RemoveFromGroup(onlineClient.ConnectionId, groupId.ToString());
            }
        }



        private async Task<ChatSessionOutput[]> GetSessions(long userId)
        {
            var groups = await chatAppService.GetUserSessions(new GetUserSessionsInput { UserId = userId });
            var dtos = groups.Items.Select(x => new ChatSessionOutput
            {
                CreationTime = x.CreationTime,
                CreatorUserId = x.CreatorUserId,
                SessionType = x.SessionType,
                Id = x.Id,
                Name = x.Name,
                Status = x.Status,
                Unread = 0,
                IsCurrent = false,
                Icon = "/images/user.png",
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

        private async Task SendOnlineUsers(WebSocketClient currentClient)
        {
            var clients = WebSocketConnectionManager.GetAllClients();
            var onlineUsers = GetOnlineUsers();
            await currentClient.WebSocket.SendMsgPackAsync(CommandType.GetOnlineUsers, onlineUsers);
            foreach (var client in clients)
            {
                if (client.UserId != currentClient.UserId)
                    await client.WebSocket.SendMsgPackAsync(CommandType.GetOnlineUsers, onlineUsers);
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
