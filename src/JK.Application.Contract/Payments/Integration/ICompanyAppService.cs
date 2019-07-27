using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using JK.Payments.Integration.Dto;

namespace JK.Payments.Integration
{
    public interface ICompanyAppService : IAsyncCrudAppService<CompanyDto, int, PagedAndSortedResultRequestDto, CompanyEditDto>
    {
        Task<CompanyEditDto> GetCompanyForEdit(EntityDto input);
    }
}