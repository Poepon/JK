using Microsoft.AspNetCore.Mvc;

namespace JK.Web.Areas.Payments.Controllers
{
    public class BanksController : PaymentsControllerBase
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}