using System;
using System.Collections.Generic;
using System.Text;
using Abp.Dependency;
using Abp.Domain.Entities.Caching;
using JK.Payments.Integration.Dto;

namespace JK.Payments.Cache
{
    public interface ICompanyCache : IEntityCache<CompanyDto>, ITransientDependency
    {
        IReadOnlyList<CompanyDto> GetActiveList();

        IReadOnlyList<CompanyDto> GetAll();
    }
}
