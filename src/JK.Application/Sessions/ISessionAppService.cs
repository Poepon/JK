using System.Threading.Tasks;
using Abp.Application.Services;
using JK.Sessions.Dto;

namespace JK.Sessions
{
    public interface ISessionAppService : IApplicationService
    {
        Task<GetCurrentLoginInformationsOutput> GetCurrentLoginInformations();
    }
}
