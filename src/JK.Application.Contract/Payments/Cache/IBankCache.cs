using System.Collections.Generic;
using Abp.Dependency;
using JK.Payments.Dto;
using Abp.Domain.Entities.Caching;
namespace JK.Payments.Cache
{
    public interface IBankCache : IEntityCache<BankDto>, ITransientDependency
    {
        BankDto Get(string code);

        IReadOnlyList<BankDto> GetAll();
    }

}