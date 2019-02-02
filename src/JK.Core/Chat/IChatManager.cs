using Abp.Domain.Services;
using System.Threading.Tasks;

namespace JK.Chat
{
    public interface IChatManager : IDomainService
    {
        Task SendMessage(int? tenantId, long userId, long groupId, string message);

        Task DeleteMessage(long messageId);

        Task<int> GetUnreadCount(long userId, long groupId);
    }
}
