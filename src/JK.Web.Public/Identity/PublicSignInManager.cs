using Abp.Dependency;
using Abp.Extensions;
using JK.Customers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace JK.Web.Public.Identity
{
    public class PublicSignInManager : SignInManager<Customer>, ITransientDependency
    {
        public PublicSignInManager(UserManager<Customer> userManager, IHttpContextAccessor contextAccessor, IUserClaimsPrincipalFactory<Customer> claimsFactory, IOptions<IdentityOptions> optionsAccessor, ILogger<SignInManager<Customer>> logger, IAuthenticationSchemeProvider schemes)
            : base(userManager, contextAccessor, claimsFactory, optionsAccessor, logger, schemes)
        {
        }
    }
}
