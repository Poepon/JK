using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using Abp.Timing;
using JK.Chat.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace JK.Chat
{
    public class ChatAppService : JKAppServiceBase, IChatAppService
    {
        private readonly IRepository<ChatMessage, long> _chatMessageRepository;
        private readonly IRepository<ChatGroup, long> _chatGroupRepository;
        private readonly IRepository<ChatGroupMember, long> _chatGrouppMemberRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IRepository<UserChatMessageLog, long> _userChatMessageLogRepository;
        public ChatAppService(IRepository<ChatMessage, long> chatMessageRepository,
            IRepository<UserChatMessageLog, long> userChatMessageLogRepository,
            IRepository<ChatGroup, long> chatGroupRepository,
            IRepository<ChatGroupMember, long> chatGrouppMemberRepository,
            IHttpContextAccessor httpContextAccessor)
        {
            _chatMessageRepository = chatMessageRepository;
            _userChatMessageLogRepository = userChatMessageLogRepository;
            _chatGroupRepository = chatGroupRepository;
            _chatGrouppMemberRepository = chatGrouppMemberRepository;
            _httpContextAccessor = httpContextAccessor;
        }


        public Task CreateGroup(CreateGroupInput input)
        {
            return _chatGroupRepository.InsertAsync(new ChatGroup
            {
                TenantId = AbpSession.TenantId,
                CreationTime = Clock.Now,
                Name = input.GroupName,
                GroupType = ChatGroupType.Public,
                CreatorUserId = input.CreatorUserId,
                IsActive = true
            });
        }

        public async Task CreatePrivate(CreatePrivateInput input)
        {
            var exists = await _chatGroupRepository.GetAll().AnyAsync(group =>
               group.GroupType == ChatGroupType.Private &&
               (group.Name == $"{input.CreatorUserId}_{input.TargetUserId}" ||
               group.Name == $"{input.TargetUserId}_{input.CreatorUserId}"),
               _httpContextAccessor.HttpContext.RequestAborted);
            if (!exists)
            {
                var groupId = await _chatGroupRepository.InsertAndGetIdAsync(new ChatGroup
                {
                    TenantId = AbpSession.TenantId,
                    Name = $"{input.CreatorUserId}_{input.TargetUserId}",
                    CreatorUserId = input.CreatorUserId,
                    GroupType = ChatGroupType.Private,
                    CreationTime = Clock.Now,
                    IsActive = true
                });
                await _chatGrouppMemberRepository.InsertAsync(new ChatGroupMember
                {
                    GroupId = groupId,
                    CreationTime = Clock.Now,
                    UserId = input.CreatorUserId
                });
                await _chatGrouppMemberRepository.InsertAsync(new ChatGroupMember
                {
                    GroupId = groupId,
                    CreationTime = Clock.Now,
                    UserId = input.TargetUserId
                });
            }
        }


        public async Task<PagedResultDto<ChatMessageDto>> GetMessages(GetMessagesInput input)
        {
            var query = _chatMessageRepository.GetAll().Where(msg => msg.GroupId == input.GroupId && msg.Id > input.LastReceivedMessageId);
            int totalCount = await query.CountAsync();
            var list = await query.OrderBy(input.Sorting).PageBy(input).ToListAsync(_httpContextAccessor.HttpContext.RequestAborted);
            return new PagedResultDto<ChatMessageDto>(totalCount, ObjectMapper.Map<List<ChatMessageDto>>(list));
        }

        public async Task JoinGroup(ChatGroupInputBase input)
        {
            var exists = await _chatGrouppMemberRepository.GetAll()
                   .AnyAsync(member => member.GroupId == input.GroupId && member.UserId == input.UserId, _httpContextAccessor.HttpContext.RequestAborted);
            if (!exists)
            {
                await _chatGrouppMemberRepository.InsertAsync(
                      new ChatGroupMember
                      {
                          GroupId = input.GroupId,
                          UserId = input.UserId,
                          CreationTime = Clock.Now
                      });
            }
        }

        public Task LeaveGroup(ChatGroupInputBase input)
        {
            return _chatGrouppMemberRepository.DeleteAsync(member => member.GroupId == input.GroupId && member.UserId == input.UserId);
        }

        public Task SendMessage(SendMessageInput input)
        {
            return _chatMessageRepository.InsertAsync(new ChatMessage
            {
                GroupId = input.GroupId,
                UserId = input.UserId,
                Message = input.Message,
                CreationTime = Clock.Now,
                ReadState = ChatMessageReadState.Unread
            });
        }

        public async Task<int> GetUnreadCount(ChatGroupInputBase input)
        {
            long lastMessageId = await _userChatMessageLogRepository.GetAll()
                .Where(log => log.GroupId == input.GroupId && log.UserId == input.UserId)
                .Select(log => log.LastReadMessageId).FirstOrDefaultAsync();
            return await _chatMessageRepository.CountAsync(message =>
                message.GroupId == input.GroupId &&
                message.Id > lastMessageId);
        }

        public async Task<long> GetLastReceivedMessageId(ChatGroupInputBase input)
        {
            var lastReceivedMessageId = await _userChatMessageLogRepository.GetAll()
                    .Where(log => log.GroupId == input.GroupId && log.UserId == input.UserId)
                    .Select(log => log.LastReceivedMessageId).SingleOrDefaultAsync();
            return lastReceivedMessageId;
        }

        public async Task<long> GetLastReadMessageId(ChatGroupInputBase input)
        {
            var lastReadMessageId = await _userChatMessageLogRepository.GetAll()
                    .Where(log => log.GroupId == input.GroupId && log.UserId == input.UserId)
                    .Select(log => log.LastReadMessageId).SingleOrDefaultAsync();
            return lastReadMessageId;
        }

        public async Task SetLastReceivedMessageId(SetLastReceivedIdInput input)
        {
            var entity = await _userChatMessageLogRepository.GetAll()
                .SingleOrDefaultAsync(log => log.GroupId == input.GroupId && log.UserId == input.UserId);
            if (entity == null)
            {
                entity = new UserChatMessageLog
                {
                    GroupId = input.GroupId,
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
                 .SingleOrDefaultAsync(log => log.GroupId == input.GroupId && log.UserId == input.UserId);
            if (entity == null)
            {
                entity = new UserChatMessageLog
                {
                    GroupId = input.GroupId,
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
    }
}
