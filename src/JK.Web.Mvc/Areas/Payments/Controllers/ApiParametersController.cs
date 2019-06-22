using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JK.Controllers;
using JK.Payments.Cache;
using JK.Web.Areas.Payments.Models.ApiParameters;
using Microsoft.AspNetCore.Mvc;

namespace JK.Web.Areas.Payments.Controllers
{
    public class ApiParametersController : PaymentsControllerBase
    {
        private readonly ICompanyCache _companyCache;
        private readonly IChannelCache _channelCache;

        public ApiParametersController(ICompanyCache companyCache,IChannelCache channelCache)
        {
            _companyCache = companyCache;
            _channelCache = channelCache;
        }
        public IActionResult Index()
        {
            var model = new ApiParametersListViewModel();
            return View(model);
        }

        public PartialViewResult CreateOrEditModal(int? id)
        {
            var model = new EditApiParametersViewModel();
            return PartialView(model);
        }
    }
}
