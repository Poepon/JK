using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JK.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace JK.Web.Areas.Payments.Controllers
{
    public class ApiParametersController : PaymentsControllerBase
    {
        public IActionResult Index()
        {
            var model = new PaymentParameterIndexViewModel(_paymentPlatformCache.GetAll(), _methodCache.GetAll());
            return View(model);
        }
    }
}
