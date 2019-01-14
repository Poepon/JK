using Microsoft.AspNetCore.Antiforgery;
using JK.Controllers;

namespace JK.Web.Host.Controllers
{
    public class AntiForgeryController : JKControllerBase
    {
        private readonly IAntiforgery _antiforgery;

        public AntiForgeryController(IAntiforgery antiforgery)
        {
            _antiforgery = antiforgery;
        }

        public void GetToken()
        {
            _antiforgery.SetCookieTokenAndHeader(HttpContext);
        }
    }
}
