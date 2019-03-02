using Abp;
using Abp.Configuration;
using Abp.Configuration.Startup;
using Abp.Domain.Services;
using Abp.Domain.Uow;
using Abp.Json;
using Abp.Localization;
using Abp.MultiTenancy;
using Abp.Runtime.Session;
using Abp.UI;
using Abp.Zero;
using Abp.Zero.Configuration;
using JK.Customers;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace JK.Web.Public.Identity
{
    public class JKUserManager<TUser, TUserLogin, TUserClaim, TUserToken> :
        UserManager<TUser>, IDomainService
        where TUser : FrontUserBase<TUserLogin, TUserClaim, TUserToken>
        where TUserLogin : FrontUserLogin, new()
        where TUserClaim : FrontUserClaim, new()
        where TUserToken : FrontUserToken, new()
    {
        public ILocalizationManager LocalizationManager { get; set; }

        protected string LocalizationSourceName { get; set; }

        public IAbpSession AbpSession { get; set; }

        protected JKUserStore<TUser, TUserLogin, TUserClaim, TUserToken> UserStore { get; }

        public IMultiTenancyConfig MultiTenancy { get; set; }

        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly ISettingManager _settingManager;
        private readonly IOptions<IdentityOptions> _optionsAccessor;

        public JKUserManager(
            JKUserStore<TUser, TUserLogin, TUserClaim, TUserToken> userStore,
            IOptions<IdentityOptions> optionsAccessor,
            IPasswordHasher<TUser> passwordHasher,
            IEnumerable<IUserValidator<TUser>> userValidators,
            IEnumerable<IPasswordValidator<TUser>> passwordValidators,
            ILookupNormalizer keyNormalizer,
            IdentityErrorDescriber errors,
            IServiceProvider services,
            ILogger<UserManager<TUser>> logger,
            IUnitOfWorkManager unitOfWorkManager,
            ISettingManager settingManager)
            : base(
                userStore,
                optionsAccessor,
                passwordHasher,
                userValidators,
                passwordValidators,
                keyNormalizer,
                errors,
                services,
                logger)
        {
            _unitOfWorkManager = unitOfWorkManager;
            _settingManager = settingManager;
            _optionsAccessor = optionsAccessor;
            UserStore = userStore;
            LocalizationManager = NullLocalizationManager.Instance;
            LocalizationSourceName = AbpZeroConsts.LocalizationSourceName;
        }

        public override async Task<IdentityResult> CreateAsync(TUser user)
        {
            var result = await CheckDuplicateUsernameOrEmailAddressAsync(user.Id, user.UserName, user.Email);
            if (!result.Succeeded)
            {
                return result;
            }

            var tenantId = GetCurrentTenantId();
            if (tenantId.HasValue && user.TenantId == 0)
            {
                user.TenantId = tenantId.Value;
            }

            var isLockoutEnabled = user.IsLockoutEnabled;
            await InitializeOptionsAsync(user.TenantId);
            var identityResult = await base.CreateAsync(user);
            if (identityResult.Succeeded)
            {
                await SetLockoutEnabledAsync(user, isLockoutEnabled);
            }

            return identityResult;
        }

        public virtual Task<TUser> FindByNameOrEmailAsync(string userNameOrEmailAddress)
        {
            return UserStore.FindByNameOrEmailAsync(userNameOrEmailAddress);
        }

        public virtual Task<List<TUser>> FindAllAsync(UserLoginInfo login)
        {
            return UserStore.FindAllAsync(login);
        }

        public virtual Task<TUser> FindAsync(int? tenantId, UserLoginInfo login)
        {
            return UserStore.FindAsync(tenantId, login);
        }

        public virtual Task<TUser> FindByNameOrEmailAsync(int? tenantId, string userNameOrEmailAddress)
        {
            return UserStore.FindByNameOrEmailAsync(tenantId, userNameOrEmailAddress);
        }

        /// <summary>
        /// Gets a user by given id.
        /// Throws exception if no user found with given id.
        /// </summary>
        /// <param name="userId">User id</param>
        /// <returns>User</returns>
        /// <exception cref="AbpException">Throws exception if no user found with given id</exception>
        public virtual async Task<TUser> GetUserByIdAsync(long userId)
        {
            var user = await FindByIdAsync(userId.ToString());
            if (user == null)
            {
                throw new AbpException("There is no user with id: " + userId);
            }

            return user;
        }

        public override async Task<IdentityResult> UpdateAsync(TUser user)
        {
            var result = await CheckDuplicateUsernameOrEmailAddressAsync(user.Id, user.UserName, user.Email);
            if (!result.Succeeded)
            {
                return result;
            }

            return await base.UpdateAsync(user);
        }

        public override async Task<IdentityResult> DeleteAsync(TUser user)
        {
            return await base.DeleteAsync(user);
        }

        public virtual async Task<IdentityResult> ChangePasswordAsync(TUser user, string newPassword)
        {
            var errors = new List<IdentityError>();

            foreach (var validator in PasswordValidators)
            {
                var validationResult = await validator.ValidateAsync(this, user, newPassword);
                if (!validationResult.Succeeded)
                {
                    errors.AddRange(validationResult.Errors);
                }
            }

            if (errors.Any())
            {
                return IdentityResult.Failed(errors.ToArray());
            }

            await UserStore.SetPasswordHashAsync(user, PasswordHasher.HashPassword(user, newPassword));
            return IdentityResult.Success;
        }

        public virtual async Task<IdentityResult> CheckDuplicateUsernameOrEmailAddressAsync(long? expectedUserId, string userName, string emailAddress)
        {
            var user = (await FindByNameAsync(userName));
            if (user != null && user.Id != expectedUserId)
            {
                throw new UserFriendlyException(string.Format(L("Identity.DuplicateUserName"), userName));
            }
            if (!string.IsNullOrEmpty(emailAddress))
            {
                user = (await FindByEmailAsync(emailAddress));
                if (user != null && user.Id != expectedUserId)
                {
                    throw new UserFriendlyException(string.Format(L("Identity.DuplicateEmail"), emailAddress));
                }
            }
            return IdentityResult.Success;
        }

        public virtual async Task InitializeOptionsAsync(int? tenantId)
        {
            Options = JsonConvert.DeserializeObject<IdentityOptions>(_optionsAccessor.Value.ToJsonString());

            //Lockout
            Options.Lockout.AllowedForNewUsers = await IsTrueAsync(AbpZeroSettingNames.UserManagement.UserLockOut.IsEnabled, tenantId);
            Options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromSeconds(await GetSettingValueAsync<int>(AbpZeroSettingNames.UserManagement.UserLockOut.DefaultAccountLockoutSeconds, tenantId));
            Options.Lockout.MaxFailedAccessAttempts = await GetSettingValueAsync<int>(AbpZeroSettingNames.UserManagement.UserLockOut.MaxFailedAccessAttemptsBeforeLockout, tenantId);

            //Password complexity
            Options.Password.RequireDigit = false;
            Options.Password.RequireLowercase = false;
            Options.Password.RequireNonAlphanumeric = false;
            Options.Password.RequireUppercase = false;
            Options.Password.RequiredLength = 6;
        }

        protected virtual Task<string> GetOldUserNameAsync(long userId)
        {
            return UserStore.GetUserNameFromDatabaseAsync(userId);
        }

        public override async Task<IList<string>> GetValidTwoFactorProvidersAsync(TUser user)
        {
            var providers = new List<string>();

            foreach (var provider in await base.GetValidTwoFactorProvidersAsync(user))
            {
                if (provider == "Email" &&
                    !await IsTrueAsync(AbpZeroSettingNames.UserManagement.TwoFactorLogin.IsEmailProviderEnabled, user.TenantId))
                {
                    continue;
                }

                if (provider == "Phone" &&
                    !await IsTrueAsync(AbpZeroSettingNames.UserManagement.TwoFactorLogin.IsSmsProviderEnabled, user.TenantId))
                {
                    continue;
                }

                providers.Add(provider);
            }

            return providers;
        }

        private bool IsTrue(string settingName, int? tenantId)
        {
            return GetSettingValue<bool>(settingName, tenantId);
        }

        private Task<bool> IsTrueAsync(string settingName, int? tenantId)
        {
            return GetSettingValueAsync<bool>(settingName, tenantId);
        }

        private T GetSettingValue<T>(string settingName, int? tenantId) where T : struct
        {
            return tenantId == null
                ? _settingManager.GetSettingValueForApplication<T>(settingName)
                : _settingManager.GetSettingValueForTenant<T>(settingName, tenantId.Value);
        }

        private Task<T> GetSettingValueAsync<T>(string settingName, int? tenantId) where T : struct
        {
            return tenantId == null
                ? _settingManager.GetSettingValueForApplicationAsync<T>(settingName)
                : _settingManager.GetSettingValueForTenantAsync<T>(settingName, tenantId.Value);
        }

        protected virtual string L(string name)
        {
            return LocalizationManager.GetString(LocalizationSourceName, name);
        }


        private int? GetCurrentTenantId()
        {
            if (_unitOfWorkManager.Current != null)
            {
                return _unitOfWorkManager.Current.GetTenantId();
            }

            return AbpSession.TenantId;
        }

        private MultiTenancySides GetCurrentMultiTenancySide()
        {
            if (_unitOfWorkManager.Current != null)
            {
                return MultiTenancy.IsEnabled && !_unitOfWorkManager.Current.GetTenantId().HasValue
                    ? MultiTenancySides.Host
                    : MultiTenancySides.Tenant;
            }

            return AbpSession.MultiTenancySide;
        }

        public virtual async Task AddTokenValidityKeyAsync(
            TUser user,
            string tokenValidityKey,
            DateTime expireDate,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            await UserStore.AddTokenValidityKeyAsync(user, tokenValidityKey, expireDate, cancellationToken);
        }

        public virtual async Task<bool> IsTokenValidityKeyValidAsync(
            TUser user,
            string tokenValidityKey,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return await UserStore.IsTokenValidityKeyValidAsync(user, tokenValidityKey, cancellationToken);
        }

        public virtual async Task RemoveTokenValidityKeyAsync(
            TUser user,
            string tokenValidityKey,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            await UserStore.RemoveTokenValidityKeyAsync(user, tokenValidityKey, cancellationToken);
        }
    }
}
