using JK.Customers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace JK.Web.Public.Identity
{
    public class JKSecurityStampValidator<TUser> : SecurityStampValidator<TUser> where TUser : class
    {
        public JKSecurityStampValidator(IOptions<SecurityStampValidatorOptions> options, SignInManager<TUser> signInManager, ISystemClock clock) : base(options, signInManager, clock)
        {
        }
    }
}
