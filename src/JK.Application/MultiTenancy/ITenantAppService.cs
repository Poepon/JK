using Abp.Application.Services;
using Abp.Application.Services.Dto;
using JK.MultiTenancy.Dto;

namespace JK.MultiTenancy
{
    public interface ITenantAppService : IAsyncCrudAppService<TenantDto, int, PagedTenantResultRequestDto, CreateTenantDto, TenantDto>
    {
    }
}

