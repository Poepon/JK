using System;
using System.Text;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Abp.AspNetCore;
using Abp.AspNetCore.Configuration;
using Abp.Configuration.Startup;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Abp.Zero.Configuration;
using JK.Authentication.JwtBearer;
using JK.Configuration;
using JK.EntityFrameworkCore;
using Abp.Runtime.Caching.Redis;
using JK.Chat.Distributed;
using Abp.RealTime;
using JK.Chat;
using JK.MultiTenancy;

namespace JK
{
    [DependsOn(
         typeof(JKApplicationModule),
         typeof(JKEntityFrameworkModule),
         typeof(AbpAspNetCoreModule),
         typeof(AbpRedisCacheModule)
     )]
    public class JKWebCoreModule : AbpModule
    {
        private readonly IHostingEnvironment _env;
        private readonly IConfigurationRoot _appConfiguration;

        public JKWebCoreModule(IHostingEnvironment env)
        {
            _env = env;
            _appConfiguration = env.GetAppConfiguration();
        }

        public override void PreInitialize()
        {
            Configuration.DefaultNameOrConnectionString = _appConfiguration.GetConnectionString(
                JKConsts.ConnectionStringName
            );

            // Use database for language management
            Configuration.Modules.Zero().LanguageManagement.EnableDbLocalization();

            Configuration.Modules.AbpAspNetCore()
                 .CreateControllersForAppServices(
                     typeof(JKApplicationModule).GetAssembly()
                 );
            if (_appConfiguration.GetValue("RedisCache:Enable", false))
            {
                Configuration.Caching.UseRedis(options =>
                {
                    options.ConnectionString = _appConfiguration["RedisCache:ConnectionString"];
                    options.DatabaseId = _appConfiguration.GetValue<int>("RedisCache:DatabaseId");
                });
            }


            ConfigureTokenAuth();

            Configuration.MultiTenancy.Resolvers.Add<MyDomainTenantResolveContributor>();
        }

        private void ConfigureTokenAuth()
        {
            IocManager.Register<TokenAuthConfiguration>();
            var tokenAuthConfig = IocManager.Resolve<TokenAuthConfiguration>();

            tokenAuthConfig.SecurityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_appConfiguration["Authentication:JwtBearer:SecurityKey"]));
            tokenAuthConfig.Issuer = _appConfiguration["Authentication:JwtBearer:Issuer"];
            tokenAuthConfig.Audience = _appConfiguration["Authentication:JwtBearer:Audience"];
            tokenAuthConfig.SigningCredentials = new SigningCredentials(tokenAuthConfig.SecurityKey, SecurityAlgorithms.HmacSha256);
            tokenAuthConfig.Expiration = TimeSpan.FromDays(1);
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(JKWebCoreModule).GetAssembly());
        }

        public override void PostInitialize()
        {
           Configuration.ReplaceService<IOnlineClientManager<ChatChannel>,ChatDistributedOnlineClientManager>();
        }
    }
}
