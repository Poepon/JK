using Abp.RealTime;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JK.Chat
{
    public class ChatChannel
    {

    }
    public interface IChatCommunicator
    {
        Task SendMessageToClient(IReadOnlyList<IOnlineClient> clients, ChatMessage message);

    }
}
