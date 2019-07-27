using Abp.Application.Services;
using JK.Payments.Integration.Dto;

namespace JK.Payments.Integration
{
    public interface IApiConfigurationAppService : ICrudAppService<ApiConfigurationDto>, IApplicationService
    {
    }
}