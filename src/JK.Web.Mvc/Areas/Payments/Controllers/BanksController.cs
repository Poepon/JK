using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using JK.Payments;
using JK.Payments.Dto;
using Microsoft.AspNetCore.Mvc;

namespace JK.Web.Areas.Payments.Controllers
{
    public class BanksController : PaymentsControllerBase
    {
        private readonly IBankAppService _bankAppService;

        public BanksController(IBankAppService bankAppService)
        {
            _bankAppService = bankAppService;
        }
        public IActionResult Index()
        {
            return View();
        }

        public async Task<PartialViewResult> CreateOrEditModal(int? id)
        {
            BankDto dto;
            if (id.HasValue)
                dto = await _bankAppService.Get(new EntityDto(id.Value));
            else
            {
                dto = new BankDto();
            }
            return PartialView("_CreateOrEditModal", dto);
        }
    }
}