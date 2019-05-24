using System;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Transactions;
using Abp.Auditing;
using Abp.Authorization;
using Abp.Configuration;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Extensions;
using Abp.IdentityFramework;
using Abp.Zero.Configuration;
using JK.Front;
using Microsoft.AspNetCore.Identity;

namespace JK.Identity
{
    public class JKLogInManager<TUser, TUserLogin, TUserClaim, TUserToken,TUserLoginAttempt> : ITransientDependency
        where TUser : FrontUserBase<TUserLogin, TUserClaim, TUserToken>
        where TUserLogin : FrontUserLogin, new()
        where TUserClaim : FrontUserClaim, new()
        where TUserToken : FrontUserToken, new()
        where TUserLoginAttempt: FrontUserLoginAttempt,new()
    {
        public IClientInfoProvider ClientInfoProvider { get; set; }
        protected IUnitOfWorkManager UnitOfWorkManager { get; }
        protected JKUserManager<TUser, TUserLogin, TUserClaim, TUserToken> UserManager { get; }
        protected ISettingManager SettingManager { get; }
        protected IRepository<TUserLoginAttempt, long> UserLoginAttemptRepository { get; }
        protected IIocResolver IocResolver { get; }

        private readonly IPasswordHasher<TUser> _passwordHasher;

        private readonly UserClaimsPrincipalFactory<TUser> _claimsPrincipalFactory;

        public JKLogInManager(
            JKUserManager<TUser, TUserLogin, TUserClaim, TUserToken> userManager,
            IUnitOfWorkManager unitOfWorkManager,
            ISettingManager settingManager,
            IRepository<TUserLoginAttempt, long> userLoginAttemptRepository,
            IIocResolver iocResolver,
            IPasswordHasher<TUser> passwordHasher,
            UserClaimsPrincipalFactory<TUser> claimsPrincipalFactory)
        {
            _passwordHasher = passwordHasher;
            _claimsPrincipalFactory = claimsPrincipalFactory;
            UnitOfWorkManager = unitOfWorkManager;
            SettingManager = settingManager;
            UserLoginAttemptRepository = userLoginAttemptRepository;
            IocResolver = iocResolver;
            UserManager = userManager;

            ClientInfoProvider = NullClientInfoProvider.Instance;
        }

        [UnitOfWork]
        public virtual async Task<JKLoginResult<TUser, TUserLogin, TUserClaim, TUserToken>> LoginAsync(UserLoginInfo login, int tenantId)
        {
            var result = await LoginAsyncInternal(login, tenantId);
            await SaveLoginAttempt(result, login.ProviderKey + "@" + login.LoginProvider,tenantId);
            return result;
        }

        protected virtual async Task<JKLoginResult<TUser, TUserLogin, TUserClaim, TUserToken>> LoginAsyncInternal(UserLoginInfo login, int tenantId)
        {
            if (login == null || login.LoginProvider.IsNullOrEmpty() || login.ProviderKey.IsNullOrEmpty())
            {
                throw new ArgumentException("login");
            }

            using (UnitOfWorkManager.Current.SetTenantId(tenantId))
            {
                var user = await UserManager.FindAsync(tenantId, login);
                if (user == null)
                {
                    return new JKLoginResult<TUser, TUserLogin, TUserClaim, TUserToken>(AbpLoginResultType.UnknownExternalLogin);
                }

                return await CreateLoginResultAsync(user);
            }
        }

        [UnitOfWork]
        public virtual async Task<JKLoginResult<TUser, TUserLogin, TUserClaim, TUserToken>> LoginAsync(string userNameOrEmailAddress, string plainPassword, int tenantId, bool shouldLockout = true)
        {
            var result = await LoginAsyncInternal(userNameOrEmailAddress, plainPassword, tenantId, shouldLockout);
            await SaveLoginAttempt(result, userNameOrEmailAddress,tenantId);
            return result;
        }

        protected virtual async Task<JKLoginResult<TUser, TUserLogin, TUserClaim, TUserToken>> LoginAsyncInternal(string userNameOrEmailAddress, string plainPassword, int tenantId, bool shouldLockout)
        {
            if (userNameOrEmailAddress.IsNullOrEmpty())
            {
                throw new ArgumentNullException(nameof(userNameOrEmailAddress));
            }

            if (plainPassword.IsNullOrEmpty())
            {
                throw new ArgumentNullException(nameof(plainPassword));
            }

            using (UnitOfWorkManager.Current.SetTenantId(tenantId))
            {
                await UserManager.InitializeOptionsAsync(tenantId);

                //TryLoginFromExternalAuthenticationSources method may create the user, that's why we are calling it before AbpUserStore.FindByNameOrEmailAsync
                var loggedInFromExternalSource = await TryLoginFromExternalAuthenticationSources(userNameOrEmailAddress, plainPassword);

                var user = await UserManager.FindByNameOrEmailAsync(tenantId, userNameOrEmailAddress);
                if (user == null)
                {
                    return new JKLoginResult<TUser, TUserLogin, TUserClaim, TUserToken>(AbpLoginResultType.InvalidUserNameOrEmailAddress);
                }

                if (await UserManager.IsLockedOutAsync(user))
                {
                    return new JKLoginResult<TUser, TUserLogin, TUserClaim, TUserToken>(AbpLoginResultType.LockedOut, user);
                }

                if (!loggedInFromExternalSource)
                {
                    if (!await UserManager.CheckPasswordAsync(user, plainPassword))
                    {
                        if (shouldLockout)
                        {
                            if (await TryLockOutAsync(tenantId, user.Id))
                            {
                                return new JKLoginResult<TUser, TUserLogin, TUserClaim, TUserToken>(AbpLoginResultType.LockedOut, user);
                            }
                        }

                        return new JKLoginResult<TUser, TUserLogin, TUserClaim, TUserToken>(AbpLoginResultType.InvalidPassword, user);
                    }

                    await UserManager.ResetAccessFailedCountAsync(user);
                }

                return await CreateLoginResultAsync(user);
            }
        }

        protected virtual async Task<JKLoginResult<TUser, TUserLogin, TUserClaim, TUserToken>> CreateLoginResultAsync(TUser user)
        {
            if (!user.IsActive)
            {
                return new JKLoginResult<TUser, TUserLogin, TUserClaim, TUserToken>(AbpLoginResultType.UserIsNotActive);
            }

            if (await IsEmailConfirmationRequiredForLoginAsync(user.TenantId) && !user.IsEmailConfirmed)
            {
                return new JKLoginResult<TUser, TUserLogin, TUserClaim, TUserToken>(AbpLoginResultType.UserEmailIsNotConfirmed);
            }

            if (await IsPhoneConfirmationRequiredForLoginAsync(user.TenantId) && !user.IsPhoneNumberConfirmed)
            {
                return new JKLoginResult<TUser, TUserLogin, TUserClaim, TUserToken>(AbpLoginResultType.UserPhoneNumberIsNotConfirmed);
            }

            var principal = await _claimsPrincipalFactory.CreateAsync(user);

            return new JKLoginResult<TUser, TUserLogin, TUserClaim, TUserToken>(
                user,
                principal.Identity as ClaimsIdentity
            );
        }

        protected virtual async Task SaveLoginAttempt(JKLoginResult<TUser, TUserLogin, TUserClaim, TUserToken> loginResult, string userNameOrEmailAddress,int tenantId)
        {
            using (var uow = UnitOfWorkManager.Begin(TransactionScopeOption.Suppress))
            {
                using (UnitOfWorkManager.Current.SetTenantId(tenantId))
                {
                    var loginAttempt = new TUserLoginAttempt
                    {
                        TenantId = tenantId,

                        UserId = loginResult.User != null ? loginResult.User.Id : (long?)null,
                        UserName = userNameOrEmailAddress,

                        Result = loginResult.Result,

                        BrowserInfo = ClientInfoProvider.BrowserInfo,
                        ClientIpAddress = ClientInfoProvider.ClientIpAddress,
                        ClientName = ClientInfoProvider.ComputerName,
                    };

                    await UserLoginAttemptRepository.InsertAsync(loginAttempt);
                    await UnitOfWorkManager.Current.SaveChangesAsync();

                    await uow.CompleteAsync();
                }
            }
        }

        protected virtual async Task<bool> TryLockOutAsync(int? tenantId, long userId)
        {
            using (var uow = UnitOfWorkManager.Begin(TransactionScopeOption.Suppress))
            {
                using (UnitOfWorkManager.Current.SetTenantId(tenantId))
                {
                    var user = await UserManager.FindByIdAsync(userId.ToString());

                    (await UserManager.AccessFailedAsync(user)).CheckErrors();

                    var isLockOut = await UserManager.IsLockedOutAsync(user);

                    await UnitOfWorkManager.Current.SaveChangesAsync();

                    await uow.CompleteAsync();

                    return isLockOut;
                }
            }
        }

        protected virtual Task<bool> TryLoginFromExternalAuthenticationSources(string userNameOrEmailAddress, string plainPassword)
        {
            return Task.FromResult(false);
        }


        protected virtual async Task<bool> IsEmailConfirmationRequiredForLoginAsync(int? tenantId)
        {
            if (tenantId.HasValue)
            {
                return await SettingManager.GetSettingValueForTenantAsync<bool>(AbpZeroSettingNames.UserManagement.IsEmailConfirmationRequiredForLogin, tenantId.Value);
            }

            return await SettingManager.GetSettingValueForApplicationAsync<bool>(AbpZeroSettingNames.UserManagement.IsEmailConfirmationRequiredForLogin);
        }

        protected virtual Task<bool> IsPhoneConfirmationRequiredForLoginAsync(int? tenantId)
        {
            return Task.FromResult(false);
        }
    }
}
