using JK.Customers;
using JK.Editions;
using JK.MultiTenancy;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JK.Web.Public.Identity
{
    public static class IdentityRegistrar
    {
        public static void Register(IServiceCollection services)
        {
            services.AddLogging();

            services.AddIdentityCore<Customer>()
           .AddUserManager<PublicUserManager>()
           .AddSignInManager<PublicSignInManager>()
           .AddUserStore<PublicUserStore>()
           .AddClaimsPrincipalFactory<PublicUserClaimsPrincipalFactory>()
           .AddDefaultTokenProviders();

            services.AddScoped<ISecurityStampValidator, PublicSecurityStampValidator>();
            services.AddScoped<PublicSecurityStampValidator>();
            //services.AddTransient<IUserPasswordStore<Customer>, PublicUserStore>();

            services.AddAuthentication(o =>
            {
                o.DefaultScheme = IdentityConstants.ApplicationScheme;
                o.DefaultSignInScheme = IdentityConstants.ExternalScheme;
            })
            .AddIdentityCookies();

        }

    }
}
