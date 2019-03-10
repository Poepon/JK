using Abp.Configuration;
using Abp.Domain.Uow;
using JK.Alliance;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;

namespace JK.Web.Public.Identity
{
    public class AgentManager : JKUserManager<Agent, AgentLogin, AgentClaim, AgentToken>
    {
        public AgentManager(AgentStore userStore, IOptions<IdentityOptions> optionsAccessor, IPasswordHasher<Agent> passwordHasher, IEnumerable<IUserValidator<Agent>> userValidators, IEnumerable<IPasswordValidator<Agent>> passwordValidators, ILookupNormalizer keyNormalizer, IdentityErrorDescriber errors, IServiceProvider services, ILogger<UserManager<Agent>> logger, IUnitOfWorkManager unitOfWorkManager, ISettingManager settingManager) : base(userStore, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger, unitOfWorkManager, settingManager)
        {
        }
    }
}
