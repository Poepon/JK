using Abp.Dependency;
using Abp.Domain.Entities.Caching;
using JK.Payments.Dto;
using JK.Payments.Enumerates;
using JK.Payments.Integration.Dto;

namespace JK.Payments.Cache
{
    public interface IResultCodeCache : IEntityCache<ResultCodeDto>, ITransientDependency
    {
        ResultCodeMean GetCodeMean(int companyId, string code);
    }
}