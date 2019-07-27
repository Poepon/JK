using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using JK.Payments.Cache;
using JK.Payments.Dto;
using JK.Payments.Integration;
using JK.Payments.Integration.Dto;
using JK.Web.Areas.Payments.Models.Companies;
using Microsoft.AspNetCore.Mvc;

namespace JK.Web.Areas.Payments.Controllers
{
    public class CompaniesController : PaymentsControllerBase
    {
        private readonly ICompanyAppService _companyAppService;
        private readonly IChannelCache _channelCache;

        public CompaniesController(ICompanyAppService companyAppService, IChannelCache channelCache)
        {
            _companyAppService = companyAppService;
            _channelCache = channelCache;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<PartialViewResult> CreateOrEditModal(int? id)
        {
            CompanyEditDto dto;
            if (id.HasValue)
                dto = await _companyAppService.GetCompanyForEdit(new EntityDto(id.Value));
            else
            {
                dto = new CompanyEditDto();
            }

            var model = new EditViewModel(dto, _channelCache.GetAll(), ObjectMapper);
            return PartialView("_CreateOrEditModal", model);
        }
    }
}