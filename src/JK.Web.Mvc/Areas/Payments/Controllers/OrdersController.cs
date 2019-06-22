using Microsoft.AspNetCore.Mvc;

namespace JK.Web.Areas.Payments.Controllers
{
    public class OrdersController : PaymentsControllerBase
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}