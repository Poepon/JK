using System.Linq;
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
                var userEmailClaim = PrincipalAccessor.Principal?.Claims.FirstOrDefault(c => c.Type == "Application_UserEmail");
                if (string.IsNullOrEmpty(userEmailClaim?.Value))
                {
                    return null;
                }

                return userEmailClaim.Value;
            }
        }
    }
}