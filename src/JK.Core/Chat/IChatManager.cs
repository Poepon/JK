using Abp.Domain.Services;
using System.Threading.Tasks;

namespace JK.Chat
{
    public interface IChatManager : IDomainService
    {

        Task DeleteMessage(long messageId);

        Task<int> GetUnreadCount(long userId, long groupId);
    }
}
