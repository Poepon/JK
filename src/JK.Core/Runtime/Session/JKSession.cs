using System.Linq;
using System.Security.Claims;
using Abp.Configuration.Startup;
using Abp.Dependency;
using Abp.MultiTenancy;
using Abp.Runtime;
using Abp.Runtime.Session;

namespace JK.Runtime.Session
{
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
    }
}