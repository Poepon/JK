using Abp.Configuration;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using JK.Customers;
using Microsoft.AspNetCore.Identity;

namespace JK.Web.Public.Identity
{
    public class CustomerLogInManager : JKLogInManager<Customer, CustomerLogin, CustomerClaim, CustomerToken, CustomerLoginAttempt>
    {
        public CustomerLogInManager(CustomerManager userManager, IUnitOfWorkManager unitOfWorkManager, ISettingManager settingManager, IRepository<CustomerLoginAttempt, long> userLoginAttemptRepository, IIocResolver iocResolver, IPasswordHasher<Customer> passwordHasher, UserClaimsPrincipalFactory<Customer> claimsPrincipalFactory) : base(userManager, unitOfWorkManager, settingManager, userLoginAttemptRepository, iocResolver, passwordHasher, claimsPrincipalFactory)
        {
        }
    }
}
