using System.Threading.Tasks;

namespace JK.Chat.Distributed
{
    public interface IDistributedHandler
    {
        Task HandleEventAsync(object eventData);
    }
}