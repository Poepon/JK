using Abp.Application.Services;
using Abp.Application.Services.Dto;
using JK.Payments.Integration.Dto.ApiParameters;

namespace JK.Payments.Integration
{
    public interface IApiParameterAppService : ICrudAppService<ApiParameterDto, int, GetParameterListInput, ApiParameterEditDto>, IApplicationService
    {
        ApiParameterEditDto GetParameterForEdit(EntityDto input);
    }

}
