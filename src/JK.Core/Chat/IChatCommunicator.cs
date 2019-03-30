using Abp.RealTime;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp;

namespace JK.Chat
{
    public interface IChatCommunicator
    {
        Task SendMessageToUser(IUserIdentifier userId, ChatMessage message);

        Task SendMessagesToUser(IUserIdentifier userId, List<ChatMessage> messages);

        Task SendSessionToUser(IUserIdentifier userId, ChatSession session);

        Task SendSessionsToUser(IUserIdentifier userId, List<ChatSession> sessions);

        Task SendOnlineClientToUser(IUserIdentifier userId, IOnlineClient onlineClient);

        Task SendOnlineClientsToUser(IUserIdentifier userId, List<IOnlineClient> onlineClients);
    }
}
