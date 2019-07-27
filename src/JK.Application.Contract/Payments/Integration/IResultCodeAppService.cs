using Abp.Application.Services;
using JK.Payments.Integration.Dto;

namespace JK.Payments.Integration
{
    public interface IResultCodeAppService : ICrudAppService<ResultCodeDto>, IApplicationService
    {
    }
}