using Abp.Domain.Repositories;
using System;
using System.Linq;
using System.Threading.Tasks;
using Abp.Timing;
using Microsoft.EntityFrameworkCore;

namespace JK.Chat
{
    public class ChatManager : IChatManager
    {
        private readonly IRepository<ChatMessage, long> _chatMessageRepository;
        private readonly IRepository<UserChatMessageLog, long> _chatMessageReadLogRepository;

        public ChatManager(IRepository<ChatMessage, long> chatMessageRepository,
            IRepository<UserChatMessageLog, long> chatMessageReadLogRepository)
        {
            _chatMessageRepository = chatMessageRepository;
            _chatMessageReadLogRepository = chatMessageReadLogRepository;
        }

        public async Task<int> GetUnreadCount(long userId, long groupId)
        {
            long lastMessageId = await _chatMessageReadLogRepository.GetAll()
                .Where(log => log.UserId == userId && log.GroupId == groupId)
                .Select(log => log.LastReadMessageId).FirstOrDefaultAsync();
            return await _chatMessageRepository.CountAsync(message =>
                message.UserId == userId &&
                message.GroupId == groupId &&
                message.Id > lastMessageId);
        }
        

        public Task DeleteMessage(long messageId)
        {
            return _chatMessageRepository.DeleteAsync(messageId);
        }

    }
}
