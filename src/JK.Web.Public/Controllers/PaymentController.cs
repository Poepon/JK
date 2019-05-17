using Abp.AspNetCore.Mvc.Controllers;
using Abp.Runtime.Session;
using Microsoft.AspNetCore.Mvc;

namespace JK.Web.Public.Controllers
{
    public class PaymentController : AbpController
    {
        public PaymentController()
        {

        }

        [Route("Pay/Callback_{CompanyId}_{ChannelId}_{AccountId}")]
        public IActionResult Callback(int CompanyId, int ChannelId, int AccountId)
        {
            int tenantId = AbpSession.GetTenantId();

            return View();
        }

      

    }
}
