using Abp;
using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using Abp.RealTime;
using Abp.Runtime.Session;
using Abp.UI;
using JK.Authorization.Users;
using JK.Chat.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace JK.Chat
{
    public class ChatAppService : JKAppServiceBase, IChatAppService
    {
        private readonly IRepository<User, long> _userRepository;
        private readonly IRepository<ChatMessage, long> _chatMessageRepository;
        private readonly IRepository<ChatSession, long> _chatSessionRepository;
        private readonly IRepository<ChatSessionMember, long> _chatSessionMemberRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IChatCommunicator _chatCommunicator;
        private readonly IOnlineClientManager<ChatChannel> _onlineClientManager;
        private readonly IRepository<UserChatMessageLog, long> _userChatMessageLogRepository;
        public ChatAppService(IRepository<ChatMessage, long> chatMessageRepository,
            IRepository<UserChatMessageLog, long> userChatMessageLogRepository,
            IRepository<ChatSession, long> chatSessionRepository,
            IRepository<ChatSessionMember, long> chatSessionMemberRepository,
            IRepository<User, long> userRepository,
            IHttpContextAccessor httpContextAccessor,
            IChatCommunicator chatCommunicator,
            IOnlineClientManager<ChatChannel> onlineClientManager)
        {
            _chatMessageRepository = chatMessageRepository;
            _userChatMessageLogRepository = userChatMessageLogRepository;
            _chatSessionRepository = chatSessionRepository;
            _chatSessionMemberRepository = chatSessionMemberRepository;
            _userRepository = userRepository;
            _httpContextAccessor = httpContextAccessor;
            _chatCommunicator = chatCommunicator;
            _onlineClientManager = onlineClientManager;
        }


        public async Task CreatePublicSession(CreatePublicSessionInput input)
        {
            var sessionId = await _chatSessionRepository.InsertAndGetIdAsync(new ChatSession
            {
                TenantId = AbpSession.TenantId,
                CreationTime = DateTimeOffset.Now.ToUnixTimeMilliseconds(),
                Name = input.SessionName,
                SessionType = ChatSessionType.Public,
                CreatorUserId = input.CreatorUserId,
                IsActive = true
            });

            await _chatSessionMemberRepository.InsertAsync(new ChatSessionMember
            {
                SessionId = sessionId,
                CreationTime = DateTimeOffset.Now.ToUnixTimeMilliseconds(),
                IsActive = true,
                UserId = input.CreatorUserId
            });
        }

        public async Task CreatePrivateSession(CreatePrivateSessionInput input)
        {
            var exists = await _chatSessionRepository.GetAll().AnyAsync(group =>
               group.SessionType == ChatSessionType.Private &&
               (group.Name == $"{input.CreatorUserId}_{input.TargetUserId}" ||
               group.Name == $"{input.TargetUserId}_{input.CreatorUserId}"),
               _httpContextAccessor.HttpContext.RequestAborted);
            if (!exists)
            {
                var sessionId = await _chatSessionRepository.InsertAndGetIdAsync(new ChatSession
                {
                    TenantId = AbpSession.TenantId,
                    Name = $"{input.CreatorUserId}_{input.TargetUserId}",
                    CreatorUserId = input.CreatorUserId,
                    SessionType = ChatSessionType.Private,
                    CreationTime = DateTimeOffset.Now.ToUnixTimeMilliseconds(),
                    IsActive = true
                });
                await _chatSessionMemberRepository.InsertAsync(new ChatSessionMember
                {
                    SessionId = sessionId,
                    CreationTime = DateTimeOffset.Now.ToUnixTimeMilliseconds(),
                    UserId = input.CreatorUserId
                });
                await _chatSessionMemberRepository.InsertAsync(new ChatSessionMember
                {
                    SessionId = sessionId,
                    CreationTime = DateTimeOffset.Now.ToUnixTimeMilliseconds(),
                    UserId = input.TargetUserId
                });
            }
        }

        public async Task<PagedResultDto<ChatMessageDto>> GetMessages(GetMessagesInput input)
        {
            var query = _chatMessageRepository.GetAllIncluding(msg => msg.User)
                .Where(msg => msg.SessionId == input.SessionId)
                .WhereIf(input.Direction == QueryDirection.New, msg => msg.Id > input.LastReceivedMessageId)
                .WhereIf(input.Direction == QueryDirection.History, msg => msg.Id <= input.LastReceivedMessageId);
            int totalCount = await query.CountAsync();
            var list = await query.OrderBy(input.Sorting).PageBy(input)
                .Select(x => new ChatMessageDto
                {
                    Id = x.Id,
                    SessionId = x.SessionId,
                    UserId = x.UserId,
                    UserName = x.User.UserName,
                    Message = x.Message,
                    CreationTime = x.CreationTime,
                    ReadState = x.ReadState
                })
                .ToListAsync();
            return new PagedResultDto<ChatMessageDto>(totalCount, list);
        }

        public async Task JoinSession(ChatSessionInputBase input)
        {
            var exists = await _chatSessionMemberRepository.GetAll()
                   .AnyAsync(member => member.SessionId == input.SessionId && member.UserId == input.UserId, _httpContextAccessor.HttpContext.RequestAborted);
            if (!exists)
            {
                await _chatSessionMemberRepository.InsertAsync(
                      new ChatSessionMember
                      {
                          SessionId = input.SessionId,
                          UserId = input.UserId,
                          CreationTime = DateTimeOffset.Now.ToUnixTimeMilliseconds()
                      });
            }
        }

        public Task LeaveSession(ChatSessionInputBase input)
        {
            return _chatSessionMemberRepository.DeleteAsync(member => member.SessionId == input.SessionId && member.UserId == input.UserId);
        }

        public async Task SendMessage(SendMessageInput input)
        {
            var message = await _chatMessageRepository.InsertAsync(new ChatMessage
            {
                SessionId = input.SessionId,
                UserId = input.UserId,
                Message = input.Message,
                CreationTime = DateTimeOffset.Now.ToUnixTimeMilliseconds(),
                ReadState = ChatMessageReadState.Unread
            });
            await CurrentUnitOfWork.SaveChangesAsync();
            // 推送消息给发送人
            var senders = _onlineClientManager.GetAllByUserId(AbpSession.ToUserIdentifier());
            await _chatCommunicator.SendMessageToClient(senders, message);
            // 推送消息给接收人
            var receivers = await GetSessionMembersUserId(input.SessionId);
            foreach (var item in receivers)
            {
                if (item != AbpSession.UserId)
                {
                    var clients = _onlineClientManager.GetAllByUserId(new UserIdentifier(AbpSession.TenantId, item));
                    await _chatCommunicator.SendMessageToClient(clients, message);
                }
            }
        }

        public async Task<long[]> GetSessionMembersUserId(long sessionId)
        {
            return await _chatSessionMemberRepository.GetAll()
                   .Where(m => m.SessionId == sessionId).Select(m => m.UserId)
                   .ToArrayAsync();
        }

        private async Task SendNewMessage(long groupId, long userId)
        {
            var lastId = await GetLastReceivedMessageId(new ChatSessionInputBase
            {
                SessionId = groupId,
                UserId = userId
            });
            var oldLastId = lastId;
            do
            {
                var list = await GetMessages(new GetMessagesInput
                {
                    SessionId = groupId,
                    UserId = userId,
                    LastReceivedMessageId = lastId,
                    MaxResultCount = 100,
                    SkipCount = 0,
                    Direction = QueryDirection.New,
                    Sorting = "Id asc"
                });
                if (list.TotalCount > 0)
                {

                }
                else
                {
                    break;
                }
            } while (true);
            if (lastId > oldLastId)
            {
                await SetLastReceivedMessageId(new SetLastReceivedIdInput
                {
                    SessionId = groupId,
                    UserId = userId,
                    LastReceivedMessageId = lastId
                });
            }
        }

        public async Task<int> GetSessionUnread(ChatSessionInputBase input)
        {
            long lastMessageId = await _userChatMessageLogRepository.GetAll()
                .Where(log => log.SessionId == input.SessionId && log.UserId == input.UserId)
                .Select(log => log.LastReadMessageId).FirstOrDefaultAsync();
            return await _chatMessageRepository.CountAsync(message =>
                message.SessionId == input.SessionId &&
                message.Id > lastMessageId);
        }
        public async Task<IList<GetSessionsUnreadOutput>> GetSessionsUnread(GetSessionsUnreadInput input)
        {
            //TODO 有BUG，没有Log的群组会查不到记录
            var linq = from g in _chatSessionRepository.GetAll()
                       join msg in _chatMessageRepository.GetAll()
                       on g.Id equals msg.SessionId
                       join log in _userChatMessageLogRepository.GetAll()
                       on g.Id equals log.SessionId
                       where log.UserId == input.UserId && msg.Id > log.LastReadMessageId
                       group g by g.Id
                      into row
                       select new GetSessionsUnreadOutput
                       {
                           SessionId = row.Key,
                           Count = row.Count()
                       };
            return await linq.ToListAsync();
        }
        public async Task<long> GetLastReceivedMessageId(ChatSessionInputBase input)
        {
            var lastReceivedMessageId = await _userChatMessageLogRepository.GetAll()
                    .Where(log => log.SessionId == input.SessionId && log.UserId == input.UserId)
                    .Select(log => log.LastReceivedMessageId).SingleOrDefaultAsync();
            return lastReceivedMessageId;
        }

        public async Task<long> GetLastReadMessageId(ChatSessionInputBase input)
        {
            var lastReadMessageId = await _userChatMessageLogRepository.GetAll()
                    .Where(log => log.SessionId == input.SessionId && log.UserId == input.UserId)
                    .Select(log => log.LastReadMessageId).SingleOrDefaultAsync();
            return lastReadMessageId;
        }

        public async Task SetLastReceivedMessageId(SetLastReceivedIdInput input)
        {
            var entity = await _userChatMessageLogRepository.GetAll()
                .SingleOrDefaultAsync(log => log.SessionId == input.SessionId && log.UserId == input.UserId);
            if (entity == null)
            {
                entity = new UserChatMessageLog
                {
                    SessionId = input.SessionId,
                    UserId = input.UserId,
                    LastReceivedMessageId = input.LastReceivedMessageId
                };
                await _userChatMessageLogRepository.InsertAsync(entity);
            }
            else
            {
                entity.LastReceivedMessageId = input.LastReceivedMessageId;
                await _userChatMessageLogRepository.UpdateAsync(entity);
            }
        }

        public async Task SetLastReadMessageId(SetLastReadIdInput input)
        {
            var entity = await _userChatMessageLogRepository.GetAll()
                 .SingleOrDefaultAsync(log => log.SessionId == input.SessionId && log.UserId == input.UserId);
            if (entity == null)
            {
                entity = new UserChatMessageLog
                {
                    SessionId = input.SessionId,
                    UserId = input.UserId,
                    LastReadMessageId = input.LastReadMessageId
                };
                await _userChatMessageLogRepository.InsertAsync(entity);
            }
            else
            {
                entity.LastReadMessageId = input.LastReadMessageId;
                await _userChatMessageLogRepository.UpdateAsync(entity);
            }
        }

        public async Task<IList<long>> GetUserSessionsId(GetUserSessionsInput input)
        {
            var list = await _chatSessionMemberRepository.GetAll()
                     .Where(member => member.UserId == input.UserId)
                     .Select(member => member.SessionId)
                     .ToListAsync();
            return list;
        }

        public async Task<ListResultDto<ChatSessionDto>> GetUserSessions(GetUserSessionsInput input)
        {
            var list = await _chatSessionMemberRepository.GetAllIncluding(member => member.ChatSession)
                      .Where(member => member.UserId == input.UserId)
                      .Select(member => member.ChatSession)
                      .ToListAsync(_httpContextAccessor.HttpContext.RequestAborted);
            var dtos = list.Select(item =>
            {
                ChatSessionDto chatSessionDto = ObjectMapper.Map<ChatSessionDto>(item);
                if (chatSessionDto.SessionType == ChatSessionType.Private)
                {
                    const string reg = @"^(?<uid1>[1-9]\d*)_(?<uid2>[1-9]\d*)$";
                    var match = Regex.Match(item.Name, reg);
                    if (match.Success)
                    {
                        var uid1 = long.Parse(match.Groups["uid1"].Value);
                        var uid2 = long.Parse(match.Groups["uid2"].Value);
                        if (uid1 != input.UserId)
                            chatSessionDto.Name = _userRepository.GetAll().Where(u => u.Id == uid1).Select(u => u.Name).SingleOrDefault();
                        if (uid2 != input.UserId)
                            chatSessionDto.Name = _userRepository.GetAll().Where(u => u.Id == uid2).Select(u => u.Name).SingleOrDefault();
                    }
                }
                return chatSessionDto;
            }).ToList();

            return new ListResultDto<ChatSessionDto>(dtos);
        }

        public Task<string> GetUserName(EntityDto<long> idDto)
        {
            return _userRepository.GetAll().Where(u => u.Id == idDto.Id).Select(u => u.Name).SingleOrDefaultAsync();
        }

        public async Task<ChatMessageDto> GetLastMessage(GetLastMessageInput input)
        {
            var entity = await _chatMessageRepository.GetAll().Where(msg => msg.SessionId == input.SessionId).OrderBy(msg => msg.Id).LastOrDefaultAsync();
            return ObjectMapper.Map<ChatMessageDto>(entity);
        }

        public async Task DeleteSession(DeleteSessionInput input)
        {
            var group = await _chatSessionRepository.GetAsync(input.SessionId);
            if (group != null)
            {
                if (group.CreatorUserId == input.UserId)
                {
                    await _chatSessionRepository.DeleteAsync(input.SessionId);
                }
                else
                {
                    throw new UserFriendlyException("您没有权限删除该聊天群组。");
                }
            }
            else
            {
                throw new UserFriendlyException("聊天群组不存在或已被删除。");
            }
            throw new NotImplementedException();
        }
    }
}
