using System;
using System.Collections.Generic;
using System.Text;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Runtime.Caching;
using JK.Payments.Integration;
using JK.Payments.Integration.Dto;

namespace JK.Payments.Cache
{
    public class CompanyCache : ICompanyCache
    {
        private readonly ICacheManager _cacheManager;
        private readonly IRepository<Company> _companyRepository;

        private const string CacheName = "";
        public CompanyCache(ICacheManager cacheManager,IRepository<Company> companyRepository)
        {
            _cacheManager = cacheManager;
            _companyRepository = companyRepository;
            _cacheManager.GetCache(CacheName);
        }
        public CompanyDto Get(int id)
        {
            throw new NotImplementedException();
        }

        public IReadOnlyList<CompanyDto> GetActiveList()
        {
            throw new NotImplementedException();
        }

        public IReadOnlyList<CompanyDto> GetAll()
        {
            throw new NotImplementedException();
        }

        public CompanyDto GetOrNull(int id)
        {
            throw new NotImplementedException();
        }
    }
}
