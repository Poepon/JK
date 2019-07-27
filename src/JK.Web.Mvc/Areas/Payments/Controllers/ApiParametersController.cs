using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using JK.Controllers;
using JK.Payments.Cache;
using JK.Payments.Integration;
using JK.Payments.Integration.Dto.ApiParameters;
using JK.Web.Areas.Payments.Models.ApiParameters;
using Microsoft.AspNetCore.Mvc;

namespace JK.Web.Areas.Payments.Controllers
{
    public class ApiParametersController : PaymentsControllerBase
    {
        private readonly ICompanyCache _companyCache;
        private readonly IChannelCache _channelCache;
        private readonly IApiParameterAppService _parameterAppService;

        public ApiParametersController(ICompanyCache companyCache,
            IChannelCache channelCache,
            IApiParameterAppService parameterAppService)
        {
            _companyCache = companyCache;
            _channelCache = channelCache;
            _parameterAppService = parameterAppService;
        }
        public IActionResult Index()
        {
            var model = new ApiParametersListViewModel();
            return View(model);
        }

        public PartialViewResult CreateOrEditModal(int? id)
        {
            var companies = _companyCache.GetAll();
            var channels = _channelCache.GetAll();
            if (id.HasValue && id.Value > 0)
            {
                var dto = _parameterAppService.GetParameterForEdit(new EntityDto(id.Value));

                var model = new EditViewModel(dto, companies, channels, ObjectMapper);
                return PartialView(model);
            }
            else
            {
                var model = new EditViewModel(new ApiParameterEditDto(), companies, channels, ObjectMapper);
                return PartialView(model);
            }
        }
    }
}
