using Abp.Application.Services.Dto;
using JK.Payments;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using JK.Payments.Dto;

namespace JK.Web.Areas.Payments.Controllers
{
    public class ChannelsController : PaymentsControllerBase
    {
        private readonly IChannelAppService _channelAppService;

        public ChannelsController(IChannelAppService channelAppService)
        {
            _channelAppService = channelAppService;
        }
        public IActionResult Index()
        {
            return View();
        }

        public async Task<PartialViewResult> CreateOrEditModal(int? id)
        {
            ChannelDto dto;
            if (id.HasValue)
                dto = await _channelAppService.Get(new EntityDto(id.Value));
            else
            {
                dto = new ChannelDto();
            }
            return PartialView("_CreateOrEditModal", dto);
        }
    }
}