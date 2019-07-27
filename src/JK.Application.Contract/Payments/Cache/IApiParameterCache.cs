using System.Collections.Generic;
using Abp.Dependency;
using Abp.Domain.Entities.Caching;
using JK.Payments.Enumerates;
using JK.Payments.Integration.Dto.ApiParameters;

namespace JK.Payments.Cache
{
    public interface IApiParameterCache :IEntityCache<ApiParameterDto>, ITransientDependency
    {
        IReadOnlyList<ApiParameterDto> GetApiParameters(int companyId,int channelId, ApiMethod method, ParameterType parameterType);
    }
}