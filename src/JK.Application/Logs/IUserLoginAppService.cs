using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using JK.Logs.Dto;

namespace JK.Logs
{
    public interface IUserLoginAppService
    {
        Task<PagedResultDto<UserLoginAttemptDto>> GetUserLoginAttempts(GetUserLoginAttemptsInput input);
    }
}
