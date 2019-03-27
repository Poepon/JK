using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Threading.Tasks;
namespace JK.Web.Public.Identity
{
    public class JKUserClaimsPrincipalFactory<TUser> : UserClaimsPrincipalFactory<TUser> where TUser : class
    {
        public JKUserClaimsPrincipalFactory(UserManager<TUser> userManager, IOptions<IdentityOptions> optionsAccessor) : base(userManager, optionsAccessor)
        {
        }

        public override Task<ClaimsPrincipal> CreateAsync(TUser user)
        {
            return base.CreateAsync(user);
        }

        protected async override Task<ClaimsIdentity> GenerateClaimsAsync(TUser user)
        {
            var id = await base.GenerateClaimsAsync(user);
            return id;
        }
    }
}
