using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using JK.Chat.Dto;

namespace JK.Chat
{
    public interface IChatAppService : IApplicationService
    {
        Task SendMessage(SendMessageInput input);

        Task<PagedResultDto<ChatMessageDto>> GetMessages(GetMessagesInput input);

        Task<ChatMessageDto> GetLastMessage(GetLastMessageInput input);

        Task CreatePrivate(CreatePrivateSessionInput input);

        Task CreateGroup(CreatePublicSessionInput input);

        Task DeleteGroup(DeleteSessionInput input);

        Task JoinGroup(ChatSessionInputBase input);

        Task LeaveGroup(ChatSessionInputBase input);

        Task<int> GetGroupUnread(ChatSessionInputBase input);

        Task<IList<GetSessionsUnreadOutput>> GetGroupsUnread(GetSessionsUnreadInput input);

        Task<long> GetLastReceivedMessageId(ChatSessionInputBase input);

        Task<long> GetLastReadMessageId(ChatSessionInputBase input);

        Task SetLastReceivedMessageId(SetLastReceivedIdInput input);

        Task SetLastReadMessageId(SetLastReadIdInput input);

        Task<IList<long>> GetUserGroupsId(GetUserSessionsInput input);

        Task<ListResultDto<ChatSessionDto>> GetUserGroups(GetUserSessionsInput input);

        Task<string> GetUserName(EntityDto<long> idDto);
    }
}
