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

        Task CreatePrivateSession(CreatePrivateSessionInput input);

        Task CreatePublicSession(CreatePublicSessionInput input);

        Task DeleteSession(DeleteSessionInput input);

        Task JoinSession(ChatSessionInputBase input);

        Task LeaveSession(ChatSessionInputBase input);

        Task<int> GetSessionUnread(ChatSessionInputBase input);

        Task<IList<GetSessionsUnreadOutput>> GetSessionsUnread(GetSessionsUnreadInput input);

        Task<long> GetLastReceivedMessageId(ChatSessionInputBase input);

        Task<long> GetLastReadMessageId(ChatSessionInputBase input);

        Task SetLastReceivedMessageId(SetLastReceivedIdInput input);

        Task SetLastReadMessageId(SetLastReadIdInput input);

        Task<IList<long>> GetUserSessionsId(GetUserSessionsInput input);

        Task<ListResultDto<ChatSessionDto>> GetUserSessions(GetUserSessionsInput input);

        Task<string> GetUserName(EntityDto<long> idDto);
    }
}
