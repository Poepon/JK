using System;
using System.Text;
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

        Task CreatePrivate(CreatePrivateInput input);

        Task CreateGroup(CreateGroupInput input);

        Task JoinGroup(ChatGroupInputBase input);

        Task LeaveGroup(ChatGroupInputBase input);

        
    }
}
