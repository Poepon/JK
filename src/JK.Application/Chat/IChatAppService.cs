using Abp.Application.Services;
using Abp.Application.Services.Dto;
using JK.Chat.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JK.Chat
{
    public interface IChatAppService : IApplicationService
    {
        Task SendMessage(SendMessageInput input);

        Task<PagedResultDto<ChatMessageDto>> GetMessages(GetMessagesInput input);

        Task<ChatMessageDto> GetLastMessage(GetLastMessageInput input);

        Task CreatePrivate(CreatePrivateInput input);

        Task CreateGroup(CreateGroupInput input);

        Task DeleteGroup(DeleteGroupInput input);

        Task JoinGroup(ChatGroupInputBase input);

        Task LeaveGroup(ChatGroupInputBase input);

        Task<int> GetUnreadCount(ChatGroupInputBase input);

        Task<long> GetLastReceivedMessageId(ChatGroupInputBase input);

        Task<long> GetLastReadMessageId(ChatGroupInputBase input);

        Task SetLastReceivedMessageId(SetLastReceivedIdInput input);

        Task SetLastReadMessageId(SetLastReadIdInput input);

        Task<IList<long>> GetUserGroupsId(GetUserGroupsInput input);

        Task<ListResultDto<ChatGroupDto>> GetUserGroups(GetUserGroupsInput input);

        Task<string> GetUserName(EntityDto<long> idDto);
    }
}
