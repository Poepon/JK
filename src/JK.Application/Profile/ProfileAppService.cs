using Abp.Authorization;
using Abp.Runtime.Caching;
using JK.Authentication.TwoFactor.Google;
using JK.Profile.Dto;
using System;
using System.Threading.Tasks;

namespace JK.Profile
{
    [AbpAuthorize]
    public class ProfileAppService : JKAppServiceBase, IProfileAppService
    {
        private const int MaxProfilPictureBytes = 1048576; //1MB
        private readonly GoogleTwoFactorAuthenticateService _googleTwoFactorAuthenticateService;
        private readonly ICacheManager _cacheManager;

        public ProfileAppService(
            GoogleTwoFactorAuthenticateService googleTwoFactorAuthenticateService,
            ICacheManager cacheManager)
        {
            _googleTwoFactorAuthenticateService = googleTwoFactorAuthenticateService;
            _cacheManager = cacheManager;
        }

        public async Task SetTwoFactorEnabled(SetTwoFactorEnabledInputDto input)
        {
            var user = await GetCurrentUserAsync();
            await UserManager.SetTwoFactorEnabledAsync(user, input.Enable);
        }

        public async Task<UpdateGoogleAuthenticatorKeyOutput> UpdateGoogleAuthenticatorKey()
        {
            var user = await GetCurrentUserAsync();
            user.GoogleAuthenticatorKey = Guid.NewGuid().ToString().Replace("-", "").Substring(0, 10);
            CheckErrors(await UserManager.UpdateAsync(user));

            return new UpdateGoogleAuthenticatorKeyOutput
            {
                QrCodeSetupImageUrl = _googleTwoFactorAuthenticateService.GenerateSetupCode(GoogleAuthenticatorConst.Issuer,
                    user.UserName, user.GoogleAuthenticatorKey, 300, 300).QrCodeSetupImageUrl
            };
        }

        public async Task<TwoFactorLoginDto> GetTwoFactorLoginForEdit()
        {
            var user = await GetCurrentUserAsync();
            var dto = new TwoFactorLoginDto
            {
                IsTwoFactorEnabled = user.IsTwoFactorEnabled,
                QrCodeSetupImageUrl = user.GoogleAuthenticatorKey != null
                ? _googleTwoFactorAuthenticateService.GenerateSetupCode(GoogleAuthenticatorConst.Issuer,
                    user.UserName, user.GoogleAuthenticatorKey, 300, 300).QrCodeSetupImageUrl
                : "",
                IsGoogleAuthenticatorEnabled = user.GoogleAuthenticatorKey != null
            };

            return dto;
        }

        public async Task ChangePassword(ChangePasswordInput input)
        {
            await UserManager.InitializeOptionsAsync(AbpSession.TenantId);

            var user = await GetCurrentUserAsync();
            CheckErrors(await UserManager.ChangePasswordAsync(user, input.CurrentPassword, input.NewPassword));
        }
    }
}
