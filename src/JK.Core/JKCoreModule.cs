using Abp.Dependency;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Abp.Timing;
using Abp.Zero;
using Abp.Zero.Configuration;
using JK.Authorization.Roles;
using JK.Authorization.Users;
using JK.Chat;
using JK.Configuration;
using JK.Localization;
using JK.MultiTenancy;
using JK.Timing;
using System.Linq;
using System.Net;

namespace JK
{
    [DependsOn(typeof(AbpZeroCoreModule))]
    public class JKCoreModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.Auditing.IsEnabledForAnonymousUsers = true;

            // Declare entity types
            Configuration.Modules.Zero().EntityTypes.Tenant = typeof(Tenant);
            Configuration.Modules.Zero().EntityTypes.Role = typeof(Role);
            Configuration.Modules.Zero().EntityTypes.User = typeof(User);

            JKLocalizationConfigurer.Configure(Configuration.Localization);

            // Enable this line to create a multi-tenant application.
            Configuration.MultiTenancy.IsEnabled = JKConsts.MultiTenancyEnabled;

            // Configure roles
            AppRoleConfig.Configure(Configuration.Modules.Zero().RoleManagement);

            Configuration.Settings.Providers.Add<AppSettingProvider>();
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(JKCoreModule).GetAssembly());
        }

        public override void PostInitialize()
        {
            IocManager.RegisterIfNot<IChatCommunicator, NullChatCommunicator>();
            var appContext = IocManager.Resolve<IAppContext>();
            appContext.LocalHostName = Dns.GetHostName();
            IocManager.Resolve<AppTimes>().StartupTime = Clock.Now;
        }
    }
}
