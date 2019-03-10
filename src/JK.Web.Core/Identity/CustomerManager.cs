using Abp.Configuration;
using Abp.Domain.Uow;
using JK.Customers;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;

namespace JK.Web.Public.Identity
{
    public class CustomerManager : JKUserManager<Customer, CustomerLogin, CustomerClaim, CustomerToken>
    {
        public CustomerManager(CustomerStore userStore, IOptions<IdentityOptions> optionsAccessor, IPasswordHasher<Customer> passwordHasher, IEnumerable<IUserValidator<Customer>> userValidators, IEnumerable<IPasswordValidator<Customer>> passwordValidators, ILookupNormalizer keyNormalizer, IdentityErrorDescriber errors, IServiceProvider services, ILogger<UserManager<Customer>> logger, IUnitOfWorkManager unitOfWorkManager, ISettingManager settingManager) : base(userStore, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger, unitOfWorkManager, settingManager)
        {
        }
    }
}
