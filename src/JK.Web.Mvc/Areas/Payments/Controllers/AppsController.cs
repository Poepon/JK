using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace JK.Web.Areas.Payments.Controllers
{
    public class AppsController : PaymentsControllerBase
    {
        public IActionResult Index()
        {
            return View();
        }
    }

    public class OrdersController : PaymentsControllerBase
    {
        public IActionResult Index()
        {
            return View();
        }
    }

    public class CompaniesController : PaymentsControllerBase
    {
        public IActionResult Index()
        {
            return View();
        }
    }

    public class ApisController : PaymentsControllerBase
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult PlaceOrderApi()
        {
            return View();
        }
    }
}
