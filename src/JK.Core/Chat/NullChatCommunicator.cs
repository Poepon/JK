using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.RealTime;

namespace JK.Chat
{
    public class NullChatCommunicator : IChatCommunicator
    {
        public Task SendMessagesToClient(IReadOnlyList<IOnlineClient> clients, List<ChatMessage> messages)
        {
            return Task.CompletedTask;
        }

        public Task SendMessageToClient(IReadOnlyList<IOnlineClient> clients, ChatMessage message)
        {
            return Task.CompletedTask;
        }

        public Task SendOnlineClientsToClient(IReadOnlyList<IOnlineClient> clients, List<IOnlineClient> onlineClients)
        {
            return Task.CompletedTask;
        }

        public Task SendOnlineClientToClient(IReadOnlyList<IOnlineClient> clients, IOnlineClient onlineClient)
        {
            return Task.CompletedTask;
        }

        public Task SendSessionsToClient(IReadOnlyList<IOnlineClient> clients, List<ChatSession> sessions)
        {
            return Task.CompletedTask;
        }

        public Task SendSessionToClient(IReadOnlyList<IOnlineClient> clients, ChatSession session)
        {
            return Task.CompletedTask;
        }
    }
}
