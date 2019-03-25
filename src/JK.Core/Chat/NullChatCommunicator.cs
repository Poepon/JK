using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.RealTime;

namespace JK.Chat
{
    public class NullChatCommunicator : IChatCommunicator
    {
        public Task SendMessageToClient(IReadOnlyList<IOnlineClient> clients, ChatMessage message)
        {
            throw new System.NotImplementedException();
        }
    }
}
