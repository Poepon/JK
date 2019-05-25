using Microsoft.AspNetCore.Mvc;

namespace JK.Web.Areas.Payments.Controllers
{
    public class ChannelsController : PaymentsControllerBase
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}