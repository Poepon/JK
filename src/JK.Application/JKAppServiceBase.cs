using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Abp.Application.Services;
using Abp.IdentityFramework;
using Abp.Runtime.Session;
using JK.Authorization.Users;
using JK.MultiTenancy;
using Abp;

namespace JK
{
    public abstract class JKServiceBase : AbpServiceBase
    {
        protected JKServiceBase()
        {
            LocalizationSourceName = JKConsts.LocalizationSourceName;
        }
    }
    /// <summary>
    /// Derive your application services from this class.
    /// </summary>
    public abstract class JKAppServiceBase : ApplicationService
    {
        public TenantManager TenantManager { get; set; }

        public UserManager UserManager { get; set; }

        protected JKAppServiceBase()
        {
            LocalizationSourceName = JKConsts.LocalizationSourceName;
        }

        protected virtual Task<User> GetCurrentUserAsync()
        {
            var user = UserManager.FindByIdAsync(AbpSession.GetUserId().ToString());
            if (user == null)
            {
                throw new Exception("There is no current user!");
            }

            return user;
        }

        protected virtual Task<Tenant> GetCurrentTenantAsync()
        {
            return TenantManager.GetByIdAsync(AbpSession.GetTenantId());
        }

        protected virtual void CheckErrors(IdentityResult identityResult)
        {
            identityResult.CheckErrors(LocalizationManager);
        }
    }
}
