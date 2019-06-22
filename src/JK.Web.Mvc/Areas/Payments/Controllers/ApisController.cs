using Microsoft.AspNetCore.Mvc;

namespace JK.Web.Areas.Payments.Controllers
{
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