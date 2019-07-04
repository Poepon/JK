using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace JK.Web.Areas.Payments.Controllers
{
    public class PaymentAppsController : PaymentsControllerBase
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
