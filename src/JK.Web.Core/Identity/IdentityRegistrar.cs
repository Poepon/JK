using JK.Customers;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace JK.Web.Public.Identity
{
    public static class IdentityRegistrar
    {
        public static void Register<TUser, TUserLogin, TUserClaim, TUserToken>
            (IServiceCollection services)
              where TUser : FrontUserBase<TUserLogin, TUserClaim, TUserToken>
              where TUserLogin : FrontUserLogin, new()
              where TUserClaim : FrontUserClaim, new()
              where TUserToken : FrontUserToken, new()
        {
            services.AddLogging();

            services.AddIdentityCore<TUser>()
           .AddUserManager<JKUserManager<TUser, TUserLogin, TUserClaim, TUserToken>>()
           .AddSignInManager<JKSignInManager<TUser>>()
           .AddUserStore<JKUserStore<TUser, TUserLogin, TUserClaim, TUserToken>>()
           .AddClaimsPrincipalFactory<JKUserClaimsPrincipalFactory<TUser>>()
           .AddDefaultTokenProviders();

            services.AddScoped<JKUserStore<TUser, TUserLogin, TUserClaim, TUserToken>>();
            services.AddScoped<ISecurityStampValidator, JKSecurityStampValidator<TUser>>();
            services.AddScoped<JKSecurityStampValidator<TUser>>();

            services.AddAuthentication(o =>
            {
                o.DefaultScheme = IdentityConstants.ApplicationScheme;
                o.DefaultSignInScheme = IdentityConstants.ExternalScheme;
            })
            .AddIdentityCookies();

        }

    }
}
