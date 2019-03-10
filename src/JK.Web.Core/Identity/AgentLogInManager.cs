using Abp.Configuration;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using JK.Alliance;
using Microsoft.AspNetCore.Identity;

namespace JK.Web.Public.Identity
{
    public class AgentLogInManager : JKLogInManager<Agent, AgentLogin, AgentClaim, AgentToken, AgentLoginAttempt>
    {
        public AgentLogInManager(AgentManager userManager, IUnitOfWorkManager unitOfWorkManager, ISettingManager settingManager, IRepository<AgentLoginAttempt, long> userLoginAttemptRepository, IIocResolver iocResolver, IPasswordHasher<Agent> passwordHasher, UserClaimsPrincipalFactory<Agent> claimsPrincipalFactory) : base(userManager, unitOfWorkManager, settingManager, userLoginAttemptRepository, iocResolver, passwordHasher, claimsPrincipalFactory)
        {
        }
    }
}
