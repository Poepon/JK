using System.Linq;
using System.Security.Claims;
using Abp.Configuration.Startup;
using Abp.Dependency;
using Abp.MultiTenancy;
using Abp.Runtime;
using Abp.Runtime.Session;

namespace JK.Runtime.Session
{
    public static class JKClaimTypes
    {
        public const string FullName = "http://jk.com/ws/2019/03/identity/claims/FullName";
    }
    public class JKSession : ClaimsAbpSession, IJKSession, ITransientDependency
    {
        public JKSession(IPrincipalAccessor principalAccessor, IMultiTenancyConfig multiTenancy, ITenantResolver tenantResolver, IAmbientScopeProvider<SessionOverride> sessionOverrideScopeProvider) : base(principalAccessor, multiTenancy, tenantResolver, sessionOverrideScopeProvider)
        {
        }

        public string UserName
        {
            get
            {
                var username = PrincipalAccessor.Principal?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name);
                if (string.IsNullOrEmpty(username?.Value))
                {
                    return null;
                }

                return username.Value;
            }
        }

        public string FullName
        {
            get
            {
                var fullName = PrincipalAccessor.Principal?.Claims.FirstOrDefault(c => c.Type == JKClaimTypes.FullName);
                if (string.IsNullOrEmpty(fullName?.Value))
                {
                    return null;
                }

                return fullName.Value;
            }
        }
    }
}