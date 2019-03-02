using JK.Customers;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
namespace JK.Web.Public.Identity
{
    public class PublicUserClaimsPrincipalFactory : UserClaimsPrincipalFactory<Customer>
    {
        public PublicUserClaimsPrincipalFactory(UserManager<Customer> userManager, IOptions<IdentityOptions> optionsAccessor) : base(userManager, optionsAccessor)
        {
        }
    }
}
