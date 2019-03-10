using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Linq;
using JK.Alliance;

namespace JK.Web.Public.Identity
{
    public class AgentStore : JKUserStore<Agent, AgentLogin, AgentClaim, AgentToken>
    {
        public AgentStore(IUnitOfWorkManager unitOfWorkManager, IRepository<Agent, long> customerRepository, IAsyncQueryableExecuter asyncQueryableExecuter, IRepository<AgentLogin, long> customerLoginRepository, IRepository<AgentClaim, long> customerClaimRepository) : base(unitOfWorkManager, customerRepository, asyncQueryableExecuter, customerLoginRepository, customerClaimRepository)
        {
        }
    }
}
