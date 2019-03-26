using Abp.RealTime;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JK.Chat
{
    public interface IChatCommunicator
    {
        Task SendMessageToClient(IReadOnlyList<IOnlineClient> clients, ChatMessage message);

        Task SendMessagesToClient(IReadOnlyList<IOnlineClient> clients, List<ChatMessage> messages);

        Task SendSessionToClient(IReadOnlyList<IOnlineClient> clients, ChatSession session);

        Task SendSessionsToClient(IReadOnlyList<IOnlineClient> clients, List<ChatSession> sessions);

        Task SendOnlineClientToClient(IReadOnlyList<IOnlineClient> clients, IOnlineClient onlineClient);

        Task SendOnlineClientsToClient(IReadOnlyList<IOnlineClient> clients, List<IOnlineClient> onlineClients);
    }
}
