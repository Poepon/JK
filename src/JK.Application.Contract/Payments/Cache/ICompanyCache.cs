using System;
using System.Collections.Generic;
using System.Text;
using Abp.Dependency;
using JK.Payments.Integration.Dto;

namespace JK.Payments.Cache
{
    public interface ICompanyCache : ITransientDependency
    {
        CompanyDto Get(int id);

        CompanyDto GetOrNull(int id);

        IReadOnlyList<CompanyDto> GetActiveList();

        IReadOnlyList<CompanyDto> GetAll();
    }
}
