using JK.Customers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace JK.Web.Public.Identity
{
    public class PublicSecurityStampValidator : SecurityStampValidator<Customer>
    {
        public PublicSecurityStampValidator(IOptions<SecurityStampValidatorOptions> options, SignInManager<Customer> signInManager, ISystemClock clock) : base(options, signInManager, clock)
        {
        }
    }
}
