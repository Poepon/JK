using Microsoft.AspNetCore.Mvc;

namespace JK.Web.Areas.Payments.Controllers
{
    public class CompaniesController : PaymentsControllerBase
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}