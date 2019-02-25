using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
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
        private readonly IRepository<ChatGroup, long> _chatGroupRepository;
        private readonly IRepository<ChatGroupMember, long> _chatGrouppMemberRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IRepository<UserChatMessageLog, long> _userChatMessageLogRepository;
        public ChatAppService(IRepository<ChatMessage, long> chatMessageRepository,
            IRepository<UserChatMessageLog, long> userChatMessageLogRepository,
            IRepository<ChatGroup, long> chatGroupRepository,
            IRepository<ChatGroupMember, long> chatGrouppMemberRepository,
            IRepository<User, long> userRepository,
            IHttpContextAccessor httpContextAccessor)
        {
            _chatMessageRepository = chatMessageRepository;
            _userChatMessageLogRepository = userChatMessageLogRepository;
            _chatGroupRepository = chatGroupRepository;
            _chatGrouppMemberRepository = chatGrouppMemberRepository;
            _userRepository = userRepository;
            _httpContextAccessor = httpContextAccessor;
        }


        public async Task CreateGroup(CreateGroupInput input)
        {
            var groupId = await _chatGroupRepository.InsertAndGetIdAsync(new ChatGroup
            {
                TenantId = AbpSession.TenantId,
                CreationTime = DateTimeOffset.Now.ToUnixTimeMilliseconds(),
                Name = input.GroupName,
                GroupType = ChatGroupType.Public,
                CreatorUserId = input.CreatorUserId,
                IsActive = true
            });

            await _chatGrouppMemberRepository.InsertAsync(new ChatGroupMember
            {
                GroupId = groupId,
                CreationTime = DateTimeOffset.Now.ToUnixTimeMilliseconds(),
                IsActive = true,
                UserId = input.CreatorUserId
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
                    CreationTime = DateTimeOffset.Now.ToUnixTimeMilliseconds(),
                    IsActive = true
                });
                await _chatGrouppMemberRepository.InsertAsync(new ChatGroupMember
                {
                    GroupId = groupId,
                    CreationTime = DateTimeOffset.Now.ToUnixTimeMilliseconds(),
                    UserId = input.CreatorUserId
                });
                await _chatGrouppMemberRepository.InsertAsync(new ChatGroupMember
                {
                    GroupId = groupId,
                    CreationTime = DateTimeOffset.Now.ToUnixTimeMilliseconds(),
                    UserId = input.TargetUserId
                });
            }
        }

        public async Task<PagedResultDto<ChatMessageDto>> GetMessages(GetMessagesInput input)
        {
            var query = _chatMessageRepository.GetAllIncluding(msg => msg.User)
                .Where(msg => msg.GroupId == input.GroupId)
                .WhereIf(input.Direction == QueryDirection.New, msg => msg.Id > input.LastReceivedMessageId)
                .WhereIf(input.Direction == QueryDirection.History, msg => msg.Id <= input.LastReceivedMessageId);
            int totalCount = await query.CountAsync();
            var list = await query.OrderBy(input.Sorting).PageBy(input)
                .Select(x => new ChatMessageDto
                {
                    Id = x.Id,
                    GroupId = x.GroupId,
                    UserId = x.UserId,
                    UserName = x.User.UserName,
                    Message = x.Message,
                    CreationTime = x.CreationTime,
                    ReadState = x.ReadState
                })
                .ToListAsync();
            return new PagedResultDto<ChatMessageDto>(totalCount, list);
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
                          CreationTime = DateTimeOffset.Now.ToUnixTimeMilliseconds()
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
                CreationTime = DateTimeOffset.Now.ToUnixTimeMilliseconds(),
                ReadState = ChatMessageReadState.Unread
            });
        }

        public async Task<int> GetGroupUnread(ChatGroupInputBase input)
        {
            long lastMessageId = await _userChatMessageLogRepository.GetAll()
                .Where(log => log.GroupId == input.GroupId && log.UserId == input.UserId)
                .Select(log => log.LastReadMessageId).FirstOrDefaultAsync();
            return await _chatMessageRepository.CountAsync(message =>
                message.GroupId == input.GroupId &&
                message.Id > lastMessageId);
        }
        public async Task<IList<GetGroupsUnreadOutput>> GetGroupsUnread(GetGroupsUnreadInput input)
        {
            //TODO 有BUG，没有Log的群组会查不到记录
            var linq = from g in _chatGroupRepository.GetAll()
                       join msg in _chatMessageRepository.GetAll()
                       on g.Id equals msg.GroupId
                       join log in _userChatMessageLogRepository.GetAll()
                       on g.Id equals log.GroupId
                       where log.UserId == input.UserId && msg.Id > log.LastReadMessageId
                       group g by g.Id
                      into row
                       select new GetGroupsUnreadOutput
                       {
                           GroupId = row.Key,
                           Count = row.Count()
                       };
            return await linq.ToListAsync();
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

        public async Task<IList<long>> GetUserGroupsId(GetUserGroupsInput input)
        {
            var list = await _chatGrouppMemberRepository.GetAll()
                     .Where(member => member.UserId == input.UserId)
                     .Select(member => member.GroupId)
                     .ToListAsync();
            return list;
        }

        public async Task<ListResultDto<ChatGroupDto>> GetUserGroups(GetUserGroupsInput input)
        {
            var list = await _chatGrouppMemberRepository.GetAllIncluding(member => member.ChatGroup)
                      .Where(member => member.UserId == input.UserId)
                      .Select(member => member.ChatGroup)
                      .ToListAsync(_httpContextAccessor.HttpContext.RequestAborted);
            var dtos = list.Select(item =>
            {
                ChatGroupDto chatGroupDto = ObjectMapper.Map<ChatGroupDto>(item);
                if (chatGroupDto.GroupType == ChatGroupType.Private)
                {
                    const string reg = @"^(?<uid1>[1-9]\d*)_(?<uid2>[1-9]\d*)$";
                    var match = Regex.Match(item.Name, reg);
                    if (match.Success)
                    {
                        var uid1 = long.Parse(match.Groups["uid1"].Value);
                        var uid2 = long.Parse(match.Groups["uid2"].Value);
                        if (uid1 != input.UserId)
                            chatGroupDto.Name = _userRepository.GetAll().Where(u => u.Id == uid1).Select(u => u.Name).SingleOrDefault();
                        if (uid2 != input.UserId)
                            chatGroupDto.Name = _userRepository.GetAll().Where(u => u.Id == uid2).Select(u => u.Name).SingleOrDefault();
                    }
                }
                return chatGroupDto;
            }).ToList();

            return new ListResultDto<ChatGroupDto>(dtos);
        }

        public Task<string> GetUserName(EntityDto<long> idDto)
        {
            return _userRepository.GetAll().Where(u => u.Id == idDto.Id).Select(u => u.Name).SingleOrDefaultAsync();
        }

        public async Task<ChatMessageDto> GetLastMessage(GetLastMessageInput input)
        {
            var entity = await _chatMessageRepository.GetAll().Where(msg => msg.GroupId == input.GroupId).OrderBy(msg => msg.Id).LastOrDefaultAsync();
            return ObjectMapper.Map<ChatMessageDto>(entity);
        }

        public async Task DeleteGroup(DeleteGroupInput input)
        {
            var group = await _chatGroupRepository.GetAsync(input.GroupId);
            if (group != null)
            {
                if (group.CreatorUserId == input.UserId)
                {
                    await _chatGroupRepository.DeleteAsync(input.GroupId);
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
