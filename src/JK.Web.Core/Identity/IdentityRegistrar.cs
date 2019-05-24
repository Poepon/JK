using JK.Alliance;
using JK.Customers;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace JK.Identity
{
    public static class AgentIdentityRegistrar
    {
        public static void Register(IServiceCollection services)
         {
            services.AddLogging();

            services.AddIdentityCore<Agent>()
           .AddUserManager<AgentManager>()
           .AddSignInManager<JKSignInManager<Agent>>()
           .AddUserStore<AgentStore>()
           .AddClaimsPrincipalFactory<JKUserClaimsPrincipalFactory<Agent>>()
           .AddDefaultTokenProviders();

            services.AddScoped<UserClaimsPrincipalFactory<Agent>, JKUserClaimsPrincipalFactory<Agent>>();
            services.AddScoped<AgentLogInManager>();

            services.AddScoped<AgentStore>();
            services.AddScoped<ISecurityStampValidator, JKSecurityStampValidator<Agent>>();
            services.AddScoped<JKSecurityStampValidator<Agent>>();

            services.AddAuthentication(o =>
            {
                o.DefaultScheme = IdentityConstants.ApplicationScheme;
                o.DefaultSignInScheme = IdentityConstants.ExternalScheme;
            })
            .AddIdentityCookies();

        }

        public static void RegisterCustomer (IServiceCollection services)
        {
            services.AddLogging();

            services.AddIdentityCore<Customer>()
           .AddUserManager<CustomerManager>()
           .AddSignInManager<JKSignInManager<Customer>>()
           .AddUserStore<CustomerStore>()
           .AddClaimsPrincipalFactory<JKUserClaimsPrincipalFactory<Customer>>()
           .AddDefaultTokenProviders();

            services.AddScoped<UserClaimsPrincipalFactory<Customer>,JKUserClaimsPrincipalFactory<Customer>>();
            services.AddScoped<CustomerLogInManager>();

            services.AddScoped<CustomerStore>();
            services.AddScoped<ISecurityStampValidator, JKSecurityStampValidator<Customer>>();
            services.AddScoped<JKSecurityStampValidator<Customer>>();

            services.AddAuthentication(o =>
            {
                o.DefaultScheme = IdentityConstants.ApplicationScheme;
                o.DefaultSignInScheme = IdentityConstants.ExternalScheme;
            })
            .AddIdentityCookies();

        }
    }
    public static class CustomerIdentityRegistrar
    {
        public static void Register(IServiceCollection services)
        {
            services.AddLogging();

            services.AddIdentityCore<Customer>()
           .AddUserManager<CustomerManager>()
           .AddSignInManager<JKSignInManager<Customer>>()
           .AddUserStore<CustomerStore>()
           .AddClaimsPrincipalFactory<JKUserClaimsPrincipalFactory<Customer>>()
           .AddDefaultTokenProviders();

            services.AddScoped<UserClaimsPrincipalFactory<Customer>, JKUserClaimsPrincipalFactory<Customer>>();
            services.AddScoped<CustomerLogInManager>();

            services.AddScoped<CustomerStore>();
            services.AddScoped<ISecurityStampValidator, JKSecurityStampValidator<Customer>>();
            services.AddScoped<JKSecurityStampValidator<Customer>>();

            services.AddAuthentication(o =>
            {
                o.DefaultScheme = IdentityConstants.ApplicationScheme;
                o.DefaultSignInScheme = IdentityConstants.ExternalScheme;
            })
            .AddIdentityCookies();

        }
    }
}
