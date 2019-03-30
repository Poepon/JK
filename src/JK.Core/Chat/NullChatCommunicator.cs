using System.Collections.Generic;
using System.Threading.Tasks;
using Abp;
using Abp.RealTime;

namespace JK.Chat
{
    public class NullChatCommunicator : IChatCommunicator
    {
        public Task SendMessagesToUser(IUserIdentifier userId, List<ChatMessage> messages)
        {
            return Task.CompletedTask;
        }

        public Task SendMessageToUser(IUserIdentifier userId, ChatMessage message)
        {
            return Task.CompletedTask;
        }

        public Task SendOnlineClientsToUser(IUserIdentifier userId, List<IOnlineClient> onlineClients)
        {
            return Task.CompletedTask;
        }

        public Task SendOnlineClientToUser(IUserIdentifier userId, IOnlineClient onlineClient)
        {
            return Task.CompletedTask;
        }

        public Task SendSessionsToUser(IUserIdentifier userId, List<ChatSession> sessions)
        {
            return Task.CompletedTask;
        }

        public Task SendSessionToUser(IUserIdentifier userId, ChatSession session)
        {
            return Task.CompletedTask;
        }
    }
}
