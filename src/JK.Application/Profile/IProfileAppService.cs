using Abp.Application.Services;
using JK.Profile.Dto;
using System.Threading.Tasks;

namespace JK.Profile
{
    public interface IProfileAppService : IApplicationService
    {
        Task ChangePassword(ChangePasswordInput input);

        Task<TwoFactorLoginDto> GetTwoFactorLoginForEdit();

        Task SetTwoFactorEnabled(SetTwoFactorEnabledInputDto input);

        Task<UpdateGoogleAuthenticatorKeyOutput> UpdateGoogleAuthenticatorKey();

    }
}
