using Abp.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace JK.Web.Public.Controllers
{
    [Route("api/pay/[action]")]
    public class PaymentApiController : AbpController
    {
        public IActionResult PlaceOrder()
        {
            return View();
        }

        public IActionResult QueryOrder()
        {
            return View();
        }

        public IActionResult CloseOrder()
        {
            return View();
        }
    }
}
