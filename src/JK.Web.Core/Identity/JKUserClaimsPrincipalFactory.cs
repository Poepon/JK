using JK.Customers;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
namespace JK.Web.Public.Identity
{
    public class JKUserClaimsPrincipalFactory<TUser> : UserClaimsPrincipalFactory<TUser> where TUser : class
    {
        public JKUserClaimsPrincipalFactory(UserManager<TUser> userManager, IOptions<IdentityOptions> optionsAccessor) : base(userManager, optionsAccessor)
        {
        }
    }
}
