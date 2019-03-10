using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Linq;
using JK.Customers;

namespace JK.Web.Public.Identity
{
    public class CustomerStore : JKUserStore<Customer, CustomerLogin, CustomerClaim, CustomerToken>
    {
        public CustomerStore(IUnitOfWorkManager unitOfWorkManager, IRepository<Customer, long> customerRepository, IAsyncQueryableExecuter asyncQueryableExecuter, IRepository<CustomerLogin, long> customerLoginRepository, IRepository<CustomerClaim, long> customerClaimRepository) : base(unitOfWorkManager, customerRepository, asyncQueryableExecuter, customerLoginRepository, customerClaimRepository)
        {
        }
    }
}
