using Abp.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace JK.Web.Public.Controllers
{
    public class PaymentController : AbpController
    {
        public PaymentController()
        {

        }

        public IActionResult Callback()
        {
            return View();
        }

      

    }
}
